using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueButtons : MonoBehaviour
{
    [Header("API Config")]
    public string apiUrl = "URL_DE_LA_API";
    
    [Header("UI References")]
    public Button[] botones;
    public TMP_Text narrativeText;
    public TMP_Text faithText;
    public Slider faithSlider;
    public GameObject loadingPanel;
    public GameObject antagPanel;
    public TMP_Text antagText;
    public float antagDisplayDuration = 10f;
    public float readingPauseDuration = 2f;
    
    [Header("Audio / Immersion")]
    public AudioSource voiceAudioSource;
    
    private int faith = 100;
    private int rivalFaith = 0;
    private int turnCounter = 0;

    private string lastRivalAction = "";
    private string lastRivalTaunt = "";
    
    private GameState gameState;
    private FaithCounter faithMonitor;

    void Start()
    {
        faithMonitor = FindFirstObjectByType<FaithCounter>();
        gameState = new GameState
        {
            player_action =
                "You open your eyes. You are a divinity. There is a village, with important and sometimes irrelevant situations to be attended. You are curious.",
            absurde_factor = 1,
            current_faith = faith,
            rival_faith = 0,
            history = new List<string>
            {
                "Turn 0: Player Action: 'You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.' -> Outcome: '' || Rival Intervention: ''"
            }
        };
        // Ensure UI starts clean
        if(antagPanel) antagPanel.SetActive(false);
        SetButtonsInteractable(false);
        StartCoroutine(SendGameStateToAPI());
    }

IEnumerator SendGameStateToAPI()
{
    SetButtonsInteractable(false);
    if (loadingPanel) loadingPanel.SetActive(true);
    narrativeText.text = "Consulting the divinity...";

    string jsonData = JsonUtility.ToJson(gameState);
    
    Debug.Log("POST to: " + apiUrl);
    Debug.Log("JsonData: "+ jsonData);
    UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
    request.SetRequestHeader("Content-Type", "application/json");

    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();

    yield return request.SendWebRequest();
    Debug.Log("Response Code: " + request.responseCode);
    if (loadingPanel) loadingPanel.SetActive(false);
    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error: " + request.error);
        narrativeText.text = "Error de conexión con el oráculo.";
    }
    else
    {
        string response = request.downloadHandler.text;
        ProcessResponse(response);
    }

    
}

void ProcessResponse(string response)
    {
        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);
        
        // Start the "Director" coroutine to show things in order
        StartCoroutine(PresentTurnSequence(apiResponse));
    }

    // --- THIS IS THE NEW SEQUENCER ---
    IEnumerator PresentTurnSequence(ApiResponse response)
    {
        // 1. Update Data (Faith, etc) immediately in background
        faith += response.antag_faith_delta;
        UpdateFaithUI();
        lastRivalAction = !string.IsNullOrEmpty(response.antag_action) ? response.antag_action : "The rival watches.";

        // 2. Show Main Narrative
        narrativeText.text = response.narrative;
        
        // OPTIONAL: Add a typewriter effect here if you want extra polish
        
        // 3. Wait for player to read the main text
        yield return new WaitForSeconds(readingPauseDuration);

        // 4. Handle Antagonist (if there is something to show)
        bool hasAntagContent = !string.IsNullOrEmpty(response.antag_taunt) || !string.IsNullOrEmpty(response.antag_action);
        
        if (hasAntagContent)
        {
            antagPanel.SetActive(true);
            
            string displayText = lastRivalAction;
            if (!string.IsNullOrEmpty(response.antag_taunt))
            {
                displayText += "\n\n\"" + response.antag_taunt + "\"";
                
                // --- VOICE INTEGRATION HERE ---
                PlayAntagonistVoice(response.antag_taunt);
            }
            antagText.text = displayText;

            // Wait for the display duration (or audio length)
            // If we have audio, we might want to wait for the clip to finish
            if (voiceAudioSource != null && voiceAudioSource.isPlaying)
            {
                yield return new WaitForSeconds(voiceAudioSource.clip.length + 1f); // Wait for audio + 1s buffer
            }
            else
            {
                yield return new WaitForSeconds(antagDisplayDuration);
            }
            
            antagPanel.SetActive(false);
        }

        // 5. Finally, Show/Enable Buttons
        SetupButtons(response.options);
    }

    // --- X&IMMERSION INTEGRATION ---
    void PlayAntagonistVoice(string textToSpeak)
    {
        if (string.IsNullOrEmpty(textToSpeak)) return;

        Debug.Log("Generating Voice for: " + textToSpeak);

        // *** PASTE YOUR X&IMMERSION CODE HERE ***
        // Example (Pseudo-code based on typical plugins):
        // XandImmersion.Speak(textToSpeak, voiceAudioSource, "VillainVoiceID");
        
        // If the plugin gives you an AudioClip, assign it:
        // voiceAudioSource.clip = generatedClip;
        // voiceAudioSource.Play();
    }

    void SetupButtons(Option[] options)
    {
        for (int i = 0; i < botones.Length; i++)
        {
            if (i < options.Length)
            {
                botones[i].gameObject.SetActive(true);
                // Reset listener to avoid stacking clicks
                botones[i].onClick.RemoveAllListeners(); 
                
                TMP_Text buttonText = botones[i].GetComponentInChildren<TMP_Text>();
                buttonText.text = options[i].desc;
                
                // Capture variable for closure
                Option opt = options[i]; 
                botones[i].onClick.AddListener(() => OnOptionSelected(opt));
            }
            else
            {
                botones[i].gameObject.SetActive(false);
            }
        }
        SetButtonsInteractable(true);
    }

    void OnOptionSelected(Option selectedOption)
    {
        faith += selectedOption.faith_delta;
        UpdateFaithUI();
        turnCounter++;
        
        string historyEntry = $"Turn {turnCounter}: Player: '{selectedOption.desc}' -> Outcome: '{selectedOption.consequence}' || Rival: '{lastRivalAction}'";
        gameState.history.Add(historyEntry);
        gameState.player_action = selectedOption.desc;
        gameState.current_faith = faith;
        gameState.absurde_factor = ProbabilityScaler.GetWeightedNumber(turnCounter);

        StartCoroutine(SendGameStateToAPI());
    }

    void UpdateFaithUI()
    {
        faith = Mathf.Clamp(faith, 0, 100); 
        faithText.text = "Faith: " + faith;
        faithSlider.value = faith;
        if (faithMonitor != null) faithMonitor.UpdateFaith(faith);
        if (faith <= 0) HandleGameOver();
    }

    void SetButtonsInteractable(bool state)
    {
        foreach(var btn in botones) btn.interactable = state;
    }

    void HandleGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}

// Data Classes (Keep these as they were)
[System.Serializable] public class GameState { public string player_action; public float absurde_factor; public int current_faith; public int rival_faith; public List<string> history; }
[System.Serializable] public class Option { public string type; public string desc; public int faith_delta; public string consequence; }
[System.Serializable] public class ApiResponse { public string narrative; public string antag_action; public string antag_taunt; public int antag_faith_delta; public Option[] options; }

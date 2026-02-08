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
    public float antagDisplayDuration = 3f;
    
    
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
                "You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.",
            absurde_factor = 1.0f,
            current_faith = faith,
            rival_faith = 0,
            history = new List<string>
            {
                "Turn 0: Player Action: 'You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.' -> Outcome: '' || Rival Intervention: ''"
            }
        };
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

    if (loadingPanel) loadingPanel.SetActive(false);
}

void ProcessResponse(string response)
{
    ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);

    lastRivalAction = !string.IsNullOrEmpty(apiResponse.antag_action) ? apiResponse.antag_action : "The rival watches.";
    lastRivalTaunt = !string.IsNullOrEmpty(apiResponse.antag_taunt) ? apiResponse.antag_taunt : "";

    narrativeText.text = apiResponse.narrative;

    faith += apiResponse.antag_faith_delta;
    UpdateFaithUI();

    if (antagPanel != null && antagText != null)
    {
        StartCoroutine(ShowAntagTextCoroutine(lastRivalAction, lastRivalTaunt));
    }

    // Asignamos las opciones a los botones
    for (int i = 0; i < botones.Length; i++)
    {
        if (i < apiResponse.options.Length)
        {
            botones[i].gameObject.SetActive(true);
            Button button = botones[i];
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            Option option = apiResponse.options[i];

            buttonText.text = option.desc;

            button.onClick.RemoveAllListeners();
                
            button.onClick.AddListener(() => OnOptionSelected(option));
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
        
    string historyEntry = $"Turn {turnCounter}: Player Action: '{selectedOption.desc}' -> Outcome: '{selectedOption.consequence}' || Rival Intervention: '{lastRivalAction}'";
        
    gameState.history.Add(historyEntry);
    gameState.player_action = selectedOption.desc;
    gameState.current_faith = faith;
    gameState.rival_faith = rivalFaith;

    Debug.Log("Historial actualizado: " + historyEntry);

    StartCoroutine(SendGameStateToAPI());
}
void UpdateFaithUI()
{
    faith = Mathf.Clamp(faith, 0, 100); 
        
    faithText.text = "Faith: " + faith;
    faithSlider.value = faith;

    if (faithMonitor != null)
    {
        faithMonitor.UpdateFaith(faith);
    }

    if (faith <= 0)
    {
        HandleGameOver();
    }
}
void SetButtonsInteractable(bool state)
{
    foreach(var btn in botones)
    {
        btn.interactable = state;
    }
}

void HandleGameOver()
{
    SceneManager.LoadScene("GameOver");
}

IEnumerator ShowAntagTextCoroutine(string action, string taunt)
{
    antagPanel.SetActive(true);
    if (!string.IsNullOrEmpty(taunt))
    {
        antagText.text = action + "\n" + taunt;
    }
    else
    {
        antagText.text = action;
    }

    yield return new WaitForSeconds(antagDisplayDuration);

    antagPanel.SetActive(false);
}
[System.Serializable]
public class GameState
{
    public string player_action;
    public float absurde_factor;
    public int current_faith;
    public int rival_faith;
    public List<string> history;
}

[System.Serializable]
public class Option
{
    public string type;
    public string desc;
    public int faith_delta;
    public string consequence;
}

[System.Serializable]
public class ApiResponse
{
    public string narrative;
    public string antag_action;
    public string antag_taunt;
    public int antag_faith_delta;
    public Option[] options;
}
}

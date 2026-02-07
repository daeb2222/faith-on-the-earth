using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class DialogueButtons : MonoBehaviour
{
    [Header("API Config")]
    public string apiUrl = "URL_DE_LA_API";  // URL de la API
    
    [Header("UI References")]
    public Button[] botones;  // Botones que quieres modificar con las opciones
    public TMP_Text narrativeText;  // Texto para mostrar la narrativa
    public TMP_Text faithText;  // Texto para mostrar la "fe"
    public Slider faithSlider;  // Slider para mostrar el porcentaje de fe
    public GameObject loadingPanel; // Opcional: para poner cosas de carga alv
    
    
    private int faith = 100;  // Valor inicial de fe
    private int rivalFaith = 0; // Necesitamos trackear esto también
    private int turnCounter = 0; // Count turns

    private string lastRivalAction = "";
    
    private GameState gameState;  // Variable para almacenar el estado del juego
    private FaithCounter faithMonitor;  // Referencia al script que maneja la fe

    void Start()
    {
        faithMonitor = FindFirstObjectByType<FaithCounter>();
        // Inicializamos el estado del juego
        gameState = new GameState
        {
            player_action =
                "You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.",
            absurde_factor = 1.0f, // TODO: randomly change the absurde factor
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
    // desactivar botones mientras piensa
    SetButtonsInteractable(false);
    if (loadingPanel) loadingPanel.SetActive(true);
    narrativeText.text = "Consulting the divinity...";

    // Convertimos el GameState a JSON
    string jsonData = JsonUtility.ToJson(gameState);
    Debug.Log("POST to: " + apiUrl);

    UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
    request.SetRequestHeader("Content-Type", "application/json");

    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();

    // Esperamos la respuesta de la API
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

//process response from api
void ProcessResponse(string response)
{
    // Deserializamos la respuesta de la API
    ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);

    // 2. Guardar datos del Rival (para usarlos en el historial del SIGUIENTE turno)
    // Asumo que antag_action viene en el JSON, si no, usa un default
    lastRivalAction = !string.IsNullOrEmpty(apiResponse.antag_action) ? apiResponse.antag_action : "The rival watches.";

    // 3. Actualizar UI Narrativa
    narrativeText.text = apiResponse.narrative;

    // 4. Actualizar Fe (Impacto del Rival)
    faith += apiResponse.antag_faith_delta;
    UpdateFaithUI();

    // Asignamos las opciones a los botones
    for (int i = 0; i < botones.Length; i++)
    {
        if (i < apiResponse.options.Length)
        {
            botones[i].gameObject.SetActive(true);
            Button button = botones[i];
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            Option option = apiResponse.options[i];

            buttonText.text = option.desc; // Mostrar descripción corta

            // Limpiar listeners viejos y poner el nuevo
            button.onClick.RemoveAllListeners();
                
            // IMPORTANTE: Aquí cerramos el loop. Pasamos la opción elegida.
            button.onClick.AddListener(() => OnOptionSelected(option));
        }
        else
        {
            botones[i].gameObject.SetActive(false); // Ocultar botones sobrantes
        }
    }
        
    // Reactivar botones para que el jugador elija
    SetButtonsInteractable(true);
}
// ESTE ES EL CORAZÓN DEL LOOP
void OnOptionSelected(Option selectedOption)
{
    // 1. Aplicar efectos inmediatos de la opción
    faith += selectedOption.faith_delta;
    UpdateFaithUI();
        
    // 2. Construir la entrada del Historial para la IA
    // Formato: "Turn X: Player Action: '...' -> Outcome: '...' || Rival Intervention: '...'"
    turnCounter++;
        
    string historyEntry = $"Turn {turnCounter}: Player Action: '{selectedOption.desc}' -> Outcome: '{selectedOption.consequence}' || Rival Intervention: '{lastRivalAction}'";
        
    // 3. Actualizar el GameState para el siguiente envío
    gameState.history.Add(historyEntry);
    gameState.player_action = selectedOption.desc; // La acción actual
    gameState.current_faith = faith;
    gameState.rival_faith = rivalFaith; // Si tuvieras lógica para esto

    Debug.Log("Historial actualizado: " + historyEntry);

    // 4. REINICIAR EL LOOP -> Llamar a la API de nuevo
    StartCoroutine(SendGameStateToAPI());
}
void UpdateFaithUI()
{
    // Clampeamos para que no pase de 0 a 100 si no quieres
    faith = Mathf.Clamp(faith, 0, 100); 
        
    faithText.text = "Faith: " + faith;
    faithSlider.value = faith;

    if (faithMonitor != null)
    {
        faithMonitor.UpdateFaith(faith);
    }
}
void SetButtonsInteractable(bool state)
{
    foreach(var btn in botones)
    {
        btn.interactable = state;
    }
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
    public string antag_action; // Añadí esto basado en tu descripción JSON
    public string antag_taunt;  // Añadí esto
    public int antag_faith_delta;
    public Option[] options;
}
}

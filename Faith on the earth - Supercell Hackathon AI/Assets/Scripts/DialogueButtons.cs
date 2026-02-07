using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class DialogueButtons : MonoBehaviour
{
    public string apiUrl = "URL_DE_LA_API";  // URL de la API
    public Button[] botones;  // Botones que quieres modificar con las opciones
    public TMP_Text narrativeText;  // Texto para mostrar la narrativa
    public TMP_Text faithText;  // Texto para mostrar la "fe"
    private int faith = 100;  // Valor inicial de fe
    private float absurdeFactor = 1.0f;  // Factor de lo absurdo
    private GameState gameState;  // Variable para almacenar el estado del juego

    void Start()
    {
        // Inicializamos el estado del juego
        gameState = new GameState
        {
            player_action = "You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.",
            absurde_factor = absurdeFactor,
            current_faith = faith,
            rival_faith = 0,
            history = new List<string>
            {
                "Turn 0: Player Action: 'You open your eyes. You are a divinity. There is a village, with small and big situations to be attended. You are curious.' -> Outcome: '' || Rival Intervention: ''"
            }
        };

        // Llamamos al m�todo para enviar el estado del juego a la API
        StartCoroutine(SendGameStateToAPI());
    }

    IEnumerator SendGameStateToAPI()
    {
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
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Network Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            yield break;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            // Procesamos la respuesta de la API
            string response = request.downloadHandler.text;
            Debug.Log("Respuesta de la API recibida: " + response); // Debug.Log para ver la respuesta de la API
            ProcessResponse(response);
        }
    }

    // Clase que representa el estado del juego
    [System.Serializable]
    public class GameState
    {
        public string player_action;
        public float absurde_factor;
        public int current_faith;
        public int rival_faith;
        public List<string> history;
    }

    // Clase que representa las opciones que recibimos de la API
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
        public Option[] options;
        public int antag_faith_delta;
    }

    // M�todo que procesa la respuesta de la API
    void ProcessResponse(string response)
    {
        // Deserializamos la respuesta de la API
        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);

        // Mostramos los valores de la narrativa y la fe
        Debug.Log("Narrativa recibida: " + apiResponse.narrative);  // Imprimir narrativa
        Debug.Log("Fe del jugador: " + faith);  // Imprimir fe del jugador

        // Actualizamos la narrativa en la UI
        narrativeText.text = apiResponse.narrative;

        // Actualizamos la fe del jugador
        faith += apiResponse.antag_faith_delta;
        faithText.text = "Faith: " + faith;

        // Mostramos los cambios en la fe
        Debug.Log("Fe del jugador despu�s de la intervenci�n del rival: " + faith);

        // Asignamos las opciones a los botones
        for (int i = 0; i < apiResponse.options.Length; i++)
        {
            if (i < botones.Length)
            {
                Button button = botones[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                Option option = apiResponse.options[i];

                // Mostramos las opciones en el log
                Debug.Log("Opci�n recibida: " + option.desc);
                Debug.Log("Fe modificada por esta opci�n: " + option.faith_delta);
                Debug.Log("Consecuencia de la opci�n: " + option.consequence);

                buttonText.text = option.desc;

                // Agregamos la acci�n al bot�n para manejar la opci�n seleccionada
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnOptionSelected(option));
            }
        }
    }

    // M�todo que se llama cuando el jugador selecciona una opci�n
    void OnOptionSelected(Option selectedOption)
    {
        // Actualizamos la fe seg�n la opci�n seleccionada
        faith += selectedOption.faith_delta;
        faithText.text = "Faith: " + faith;

        // Mostrar la consecuencia de la opci�n seleccionada (puedes usarla de manera creativa)
        Debug.Log("Opci�n seleccionada: " + selectedOption.desc);
        Debug.Log("Consecuencia de la opci�n: " + selectedOption.consequence);
        Debug.Log("Fe despu�s de la opci�n seleccionada: " + faith);
    }
}

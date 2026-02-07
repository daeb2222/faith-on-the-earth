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
    public Slider faithSlider;  // Slider para mostrar el porcentaje de fe

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

        // Llamamos al método para enviar el estado del juego a la API
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

    // Método que procesa la respuesta de la API
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
        Debug.Log("Fe del jugador después de la intervención del rival: " + faith);

        // Actualizamos el Slider para que reaccione al porcentaje de fe
        UpdateFaithSlider();

        // Asignamos las opciones a los botones
        for (int i = 0; i < apiResponse.options.Length; i++)
        {
            if (i < botones.Length)
            {
                Button button = botones[i];
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                Option option = apiResponse.options[i];

                // Mostramos las opciones en el log
                Debug.Log("Opción recibida: " + option.desc);
                Debug.Log("Fe modificada por esta opción: " + option.faith_delta);
                Debug.Log("Consecuencia de la opción: " + option.consequence);

                buttonText.text = option.desc;

                // Agregamos la acción al botón para manejar la opción seleccionada
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnOptionSelected(option));
            }
        }
    }

    // Método que actualiza el Slider con el porcentaje de fe
    void UpdateFaithSlider()
    {
        // Convertimos la fe en un porcentaje para el slider (0 a 100)
        faithSlider.value = faith; // Suponiendo que el Slider va de 0 a 100
    }

    // Método que se llama cuando el jugador selecciona una opción
    void OnOptionSelected(Option selectedOption)
    {
        // Actualizamos la fe según la opción seleccionada
        faith += selectedOption.faith_delta;
        faithText.text = "Faith: " + faith;

        // Mostrar la consecuencia de la opción seleccionada (puedes usarla de manera creativa)
        Debug.Log("Opción seleccionada: " + selectedOption.desc);
        Debug.Log("Consecuencia de la opción: " + selectedOption.consequence);
        Debug.Log("Fe después de la opción seleccionada: " + faith);

        // Actualizamos el Slider después de cambiar la fe
        UpdateFaithSlider();
    }
}

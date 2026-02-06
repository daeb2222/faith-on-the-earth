using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad de movimiento de la cámara
    public float zoomSpeed = 10f; // Velocidad de zoom
    public float minZoom = 5f;    // Zoom mínimo
    public float maxZoom = 20f;   // Zoom máximo
    public Vector3 initialOffset = new Vector3(0, 10, -10); // Posición inicial de la cámara para la vista isométrica

    private Camera mainCamera;

    // Referencias a las acciones del Input System
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction rightClickAction;

    // Cargar el InputActionAsset que has creado
    public InputActionAsset inputActionAsset;

    void Awake()
    {
        mainCamera = Camera.main;

        // Cargar las acciones de input desde el InputActionAsset
        var actionMap = inputActionAsset.FindActionMap("Player");

        // Obtener las acciones del Input System
        moveAction = actionMap.FindAction("Navigate");
        rightClickAction = actionMap.FindAction("RightClickMouse");
        zoomAction = actionMap.FindAction("ScrollWheel");

        // Activar las acciones
        moveAction.Enable();
        rightClickAction.Enable();
        zoomAction.Enable();
    }

    void Start()
    {
        // Configurar la posición inicial de la cámara
        transform.position = initialOffset;
    }

    void Update()
    {
        // Solo mover la cámara si el botón derecho del ratón está presionado
        if (rightClickAction.ReadValue<float>() > 0) // Si el clic derecho está presionado
        {
            HandleMove();
        }

        HandleZoom();
    }

    private void HandleMove()
    {
        // Obtener la entrada de movimiento desde el ratón (delta del mouse)
        Vector2 moveInput = moveAction.ReadValue<Vector2>(); // Lee el movimiento del ratón
        float horizontal = moveInput.x * moveSpeed * Time.deltaTime;
        float vertical = moveInput.y * moveSpeed * Time.deltaTime;

        // Aplicar el movimiento de la cámara
        transform.Translate(-horizontal, 0, -vertical, Space.World);  // Mover la cámara en el espacio global
    }

    private void HandleZoom()
    {
        // Obtener el valor del scroll para el zoom
        float zoomInput = zoomAction.ReadValue<float>();
        float zoom = zoomInput * zoomSpeed;

        // Calcular y aplicar el zoom
        float newZoom = Mathf.Clamp(transform.position.y - zoom, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, newZoom, transform.position.z);
    }

    private void OnDisable()
    {
        // Desactivar las acciones cuando no se usen
        moveAction.Disable();
        zoomAction.Disable();
        rightClickAction.Disable();
    }
}

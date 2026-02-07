using UnityEngine;

public class SmoothOrbitCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Configuración de Órbita")]
    public float distance = 25f;
    public float minDistance = 10f;
    public float maxDistance = 60f;
    
    [Header("Sensibilidad y Suavizado (La clave del 'Game Feel')")]
    public float xSpeed = 250f;  // Velocidad horizontal
    public float ySpeed = 120f;  // Velocidad vertical
    public float smoothTime = 0.2f; // Cuánto tarda en frenar (0 = rígido, 0.5 = muy suave)
    public float zoomDampening = 5f; // Suavizado del zoom

    [Header("Límites Verticales")]
    public float yMinLimit = -10f; // No bajar mucho
    public float yMaxLimit = 80f;  // No subir demasiado

    [Header("Auto-Rotación (Idle)")]
    public float autoRotateSpeed = 5f;
    public float timeToIdle = 2f; // Segundos antes de empezar a girar solo

    [Header("Levitación")]
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    // Variables internas para el suavizado
    private float xTarget, yTarget; 
    private float xCurrent, yCurrent;
    private float xVelocity, yVelocity; // Referencias para SmoothDamp
    private float currentDistance;
    private float lastInteractionTime;

    void Start()
    {
        // Configuración inicial
        if (target == null)
        {
            GameObject center = new GameObject("WorldCenter");
            target = center.transform;
        }

        Vector3 angles = transform.eulerAngles;
        xTarget = angles.y;
        yTarget = angles.x;
        
        // Inicializamos las actuales para que no salte al play
        xCurrent = xTarget;
        yCurrent = yTarget;
        currentDistance = distance;
        
        lastInteractionTime = Time.time;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1. Detectar Input del Mouse
        if (Input.GetMouseButton(0))
        {
            // Acumulamos el input en los "Targets" (Objetivos)
            xTarget += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yTarget -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            yTarget = ClampAngle(yTarget, yMinLimit, yMaxLimit);
            
            lastInteractionTime = Time.time; // Reseteamos el timer de inactividad
        }
        else
        {
            // 2. Lógica de Idle (Auto-giro)
            if (Time.time - lastInteractionTime > timeToIdle)
            {
                xTarget += autoRotateSpeed * Time.deltaTime;
            }
        }

        // 3. Input de Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            distance -= scroll * 10f; // Velocidad base del scroll
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            lastInteractionTime = Time.time;
        }

        // --- LA MAGIA DEL SUAVIZADO ---
        
        // Usamos SmoothDamp para interpolar suavemente desde la rotación actual a la objetivo
        xCurrent = Mathf.SmoothDamp(xCurrent, xTarget, ref xVelocity, smoothTime);
        yCurrent = Mathf.SmoothDamp(yCurrent, yTarget, ref yVelocity, smoothTime);
        
        // Suavizamos también la distancia (Zoom suave)
        currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * zoomDampening);

        // 4. Calcular Rotación y Posición
        Quaternion rotation = Quaternion.Euler(yCurrent, xCurrent, 0);

        // Efecto de Levitación (Seno)
        float levitationOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        
        // Posición final: Centro - (Dirección * Distancia) + Altura + Levitación
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
        Vector3 position = rotation * negDistance + target.position;
        
        position.y += levitationOffset; // Añadimos la levitación al final

        // Aplicar transformaciones
        transform.rotation = rotation;
        transform.position = position;
    }

    // Función auxiliar para limitar ángulos correctamente (evita el problema de 360 a 0)
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
using UnityEngine;

public class NPCSAnimator : MonoBehaviour
{
    public float moveSpeed = 3f;  // Velocidad de movimiento
    public float prayTime = 2f;   // Tiempo que el NPC pasa rezando
    public float walkTime = 2f;   // Tiempo que el NPC pasa caminando
    public Animator animator;     // Animator que controla las animaciones

    private Rigidbody rb;  // Componente Rigidbody
    private bool isPraying = false;  // Estado de oración
    private float stateTimer = 0f;  // Temporizador para alternar entre rezar y caminar
    private Vector3 lastPosition;   // Última posición cuando el NPC estaba caminando

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Obtener el Rigidbody
        if (animator == null) animator = GetComponent<Animator>();  // Obtener el Animator
        lastPosition = transform.position;  // Inicializar la última posición
    }

    void Update()
    {
        // Temporizador que alterna entre rezar y caminar
        stateTimer += Time.deltaTime;

        if (isPraying)
        {
            if (stateTimer >= prayTime)
            {
                // Cambiar a caminar después de 'prayTime' segundos
                isPraying = false;
                stateTimer = 0f;
                animator.SetBool("IsPraying", false);
                animator.SetBool("IsWalking", true);  // Activar animación de caminar
            }
        }
        else
        {
            if (stateTimer >= walkTime)
            {
                // Cambiar a rezar después de 'walkTime' segundos
                isPraying = true;
                stateTimer = 0f;
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsPraying", true);  // Activar animación de rezar
            }
        }

        // Movimiento automático
        MoveNPC();
    }

    void MoveNPC()
    {
        if (isPraying)
        {
            // El NPC se queda quieto cuando está rezando, pero mantiene su última posición
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            // Movimiento hacia adelante mientras camina
            Vector3 forwardMovement = transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + forwardMovement);  // Mover al NPC

            lastPosition = transform.position;  // Actualizar la última posición al moverse
        }
    }
}

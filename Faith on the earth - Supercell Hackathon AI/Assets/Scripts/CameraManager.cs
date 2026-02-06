using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 100f;

    public Transform startTransform;
    public Vector2 xBounds = new Vector2(-50f, 50f); 
    public Vector2 yBounds = new Vector2(-10f, 10f); 

    private Vector3 lastMousePosition;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (startTransform != null)
        {
            transform.position = startTransform.position;
        }
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Rotate(Vector3.up, -delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotateSpeed * Time.deltaTime, Space.World);
        }

        lastMousePosition = Input.mousePosition;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = Mathf.Clamp(cam.fieldOfView - scroll * zoomSpeed, minZoom, maxZoom);
        cam.fieldOfView = newZoom;

        float moveX = transform.position.x + Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float moveY = transform.position.y + Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        moveX = Mathf.Clamp(moveX, xBounds.x, xBounds.y);
        moveY = Mathf.Clamp(moveY, yBounds.x, yBounds.y);

        transform.position = new Vector3(moveX, moveY, transform.position.z);
    }
}

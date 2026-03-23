using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivity = 100f;

    [Header("Referencias")]
    public Transform playerBody; // el objeto Player
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // bloquea el cursor
    }

    void Update()
    {
        // Obtener movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical (mirar arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // limita la rotación vertical

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (gira al jugador)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

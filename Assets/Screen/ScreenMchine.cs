using Aya.UNES;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ScreenMachine : MonoBehaviour
{
    [Header("Emulador y Render")]
    public UNESBehaviour emulator;
    public Renderer screenRenderer;
    public TextAsset defaultROM;

    [Header("Player Setup")]
    public GameObject player;
    public MonoBehaviour playerController; // script de movimiento
    public Animator playerAnimator;
    public Transform interactionPoint;     // punto fijo delante del screen

    [Header("World Timers")]
    public TextMesh currentRunTimerMesh; // 3D text para timer jugador
    public TextMesh recordTimerMesh;     // 3D text para record

    private bool hasBooted = false;
    private bool playerInside = false;
    private bool isUsingEmulator = false;
    private float currentRunTime = 0f;
    private float recordTime = 999f; // ejemplo inicial muy alto

    void Start()
    {
        if (emulator == null)
        {
            Debug.LogError("[ScreenMachine] No se asignó UNESBehaviour!");
            return;
        }

        // Asignar RenderTexture al quad
        if (screenRenderer != null)
            screenRenderer.material.mainTexture = emulator.RenderTexture;

        // Asegurar trigger
        var collider = GetComponent<BoxCollider>();
        if (!collider.isTrigger) collider.isTrigger = true;

        // Inicializar textos 3D
        if (currentRunTimerMesh != null) currentRunTimerMesh.text = "00:00.000";
        if (recordTimerMesh != null) recordTimerMesh.text = "RECORD: 00:00.000";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("[ScreenMachine] Jugador entró.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("[ScreenMachine] Jugador salió.");
        }
    }

    void Update()
    {
        HandleInteraction();
        HandleExit();
        UpdateWorldTimers();
    }

    private void HandleInteraction()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Boot ROM solo una vez
            if (!hasBooted && defaultROM != null)
            {
                emulator.Boot(defaultROM.bytes);
                hasBooted = true;
                Debug.Log("[ScreenMachine] ROM iniciada.");
            }

            // Activar modo emulador
            isUsingEmulator = true;

            // Mover jugador al punto fijo
            if (interactionPoint != null && player != null)
            {
                player.transform.position = interactionPoint.position;
                player.transform.rotation = interactionPoint.rotation;
            }

            // Bloquear movimiento
            if (playerController != null)
                playerController.enabled = false;

            // Activar animación
            if (playerAnimator != null)
                playerAnimator.SetBool("IsPlaying", true);

            // Reset del timer del jugador
            currentRunTime = 0f;
        }
    }

    private void HandleExit()
    {
        if (isUsingEmulator && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("[ScreenMachine] Saliendo del emulador.");

            // Restaurar control
            if (playerController != null)
                playerController.enabled = true;

            // Parar animación
            if (playerAnimator != null)
                playerAnimator.SetBool("IsPlaying", false);

            // Recargar escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateWorldTimers()
    {
        if (!isUsingEmulator || !hasBooted) return;

        // Incrementar timer del jugador
        currentRunTime += Time.deltaTime;

        // Leer progreso de la ROM (Super Mario NES)
        byte[] mem = emulator.GetMemory(); // memoria expuesta como byte[]
        if (mem != null && mem.Length > 0x075D)
        {
            // 0x075D == 0x01 indica que se completó el nivel
            if (mem[0x075D] == 0x01)
            {
                isUsingEmulator = false; // detiene timer

                // Guardar récord si se hizo mejor tiempo
                if (currentRunTime < recordTime)
                {
                    recordTime = currentRunTime;
                    Debug.Log("[ScreenMachine] ¡NEW RECORD!  " + recordTime.ToString("F2") + "s");
                }
            }
        }

        // Actualizar TextMesh en la escena
        if (currentRunTimerMesh != null)
            currentRunTimerMesh.text = $"Time: {FormatTime(currentRunTime)}";

        if (recordTimerMesh != null)
            recordTimerMesh.text = $"RECORD: {FormatTime(recordTime)}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int millis = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, millis);
    }
    // Método separado para manejar recarga del emulador
    private void HandleEmulatorReload()
    {
        // Solo permitimos recargar si el jugador está dentro y el emulador ya ha sido bootado
        if (playerInside && hasBooted)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("[ScreenMachine] Recargando escena desde el emulador.");

                // Aquí podrías desactivar input si implementas SetInputActive
                // emulator.SetInputActive(false);

                // Recargar la escena actual
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
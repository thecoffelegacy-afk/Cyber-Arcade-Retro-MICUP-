using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeEmulatorManager : MonoBehaviour
{
    // Canvas con InputField + Button
    public GameObject emulatorUI;

    void Start()
    {
        // La UI empieza oculta
        if (emulatorUI != null)
            emulatorUI.SetActive(false);

        // Cursor bloqueado por defecto
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
       
    }

    // Método público para abrir la UI del emulador
    public void OpenEmulator()
    {
        if (emulatorUI != null)
            emulatorUI.SetActive(true);

        // Mostrar cursor y desbloquearlo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
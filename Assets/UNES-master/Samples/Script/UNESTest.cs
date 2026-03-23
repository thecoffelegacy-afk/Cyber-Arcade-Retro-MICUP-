using Aya.UNES;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UNES_Test : MonoBehaviour
{
    private UNESBehaviour emulator; // ahora privado

    public InputField romInputField;
    public Button loadButton;


    void Start()
    {
        // Tomar automáticamente el UNESBehaviour del mismo GameObject
        emulator = GetComponent<UNESBehaviour>();
        if (emulator == null)
        {
            Debug.LogError("[UNES_Test] No se encontró UNESBehaviour en este GameObject!");
            return;
        }

        // Asociar evento al botón
        loadButton.onClick.AddListener(LoadROMFromInput);
    }

    void LoadROMFromInput()
    {
        string path = romInputField.text;

        if (File.Exists(path))
        {
            byte[] romData = File.ReadAllBytes(path);
            emulator.Boot(romData);

            Debug.Log("[UNES_Test] ROM cargada: " + path);

            // Una vez cargada la ROM, ocultar la UI
            romInputField.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("[UNES_Test] Archivo no encontrado: " + path);
        }
    }
}
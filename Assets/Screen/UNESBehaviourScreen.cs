using UnityEngine;
using Aya.UNES;

[RequireComponent(typeof(UNESBehaviour))]
public class UNESBehaviourScreen : MonoBehaviour
{
    [Header("Render")]
    public Renderer screenRenderer; // Quad donde se verá la ROM

    private UNESBehaviour emulator; // Instancia propia del emulador

    private bool _inputActive = false; // controla si el emulador procesa input

    void Awake()
    {
        // Tomamos el UNESBehaviour del mismo GameObject
        emulator = GetComponent<UNESBehaviour>();
        if (emulator == null)
        {
            Debug.LogError("[UNESBehaviourScreen] No se encontró UNESBehaviour en este GameObject.");
            return;
        }

        // Crear RenderTexture propia si no existe
        if (emulator.RenderTexture == null)
        {
            emulator.RenderTexture = new RenderTexture(UNESBehaviour.GameWidth, UNESBehaviour.GameHeight, 0);
            emulator.RenderTexture.filterMode = FilterMode.Point;
        }

        // Asignar la RenderTexture al material del quad
        if (screenRenderer != null)
        {
            // Material único para este quad
            screenRenderer.material = new Material(screenRenderer.material);
            screenRenderer.material.mainTexture = emulator.RenderTexture;
        }
    }

    void Update()
    {
        if (emulator == null) return;

        // Solo procesamos input si está activo
        if (_inputActive)
        {
            emulator.UpdateInput();
        }

        // Renderizamos el frame siempre
        emulator.UpdateRender();
    }

    /// <summary>
    /// Activa o desactiva el input para este emulador.
    /// Llamar desde ScreenMachine cuando el jugador pulse E dentro del área.
    /// </summary>
    public void SetInputActive(bool active)
    {
        _inputActive = active;
    }
}
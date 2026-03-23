using UnityEngine;

public class GBScreen : MonoBehaviour
{
    public static GBScreen Instance; // singleton para acceso fácil
    private Renderer quadRenderer;

    private void Awake()
    {
        Instance = this;
        quadRenderer = GetComponent<Renderer>();
    }

    // Llamar desde Drawer cuando llegue la textura remota
    public void SetTexture(Texture2D tex)
    {
        quadRenderer.material.mainTexture = tex;
    }
}
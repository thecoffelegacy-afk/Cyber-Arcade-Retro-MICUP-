using UnityEngine;
using Photon.Pun;
using GB.Graphics;

public class GBEmulatorStreamer : MonoBehaviourPun
{
    public Drawer drawer;              // referencia al Drawer
    public Renderer quadRenderer;      // quad visible para todos

    private Texture2D receivedTexture;

    void Start()
    {
        InvokeRepeating(nameof(SendFrame), 0f, 0.15f); // ~6 FPS
    }

    void SendFrame()
    {
        if (!IsLocalUsing()) return;

        Texture2D tex = drawer.GetTexture();
        if (tex == null) return;

        byte[] data = tex.EncodeToJPG(30);

        photonView.RPC(nameof(ReceiveFrame), RpcTarget.Others, data);
    }

    bool IsLocalUsing()
    {
        ArcadeMchineGB machine = GetComponent<ArcadeMchineGB>();
        return machine != null && machine.IsBeingUsedByLocalPlayer();
    }

    [PunRPC]
    void ReceiveFrame(byte[] data)
    {
        if (receivedTexture == null)
            receivedTexture = new Texture2D(2, 2);

        receivedTexture.LoadImage(data);

        quadRenderer.material.mainTexture = receivedTexture;
    }
}
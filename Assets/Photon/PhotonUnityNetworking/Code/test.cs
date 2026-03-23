using UnityEngine;
using Photon.Pun;

public class TestPhotonConnection : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Conecta usando el App ID que pegaste en PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Intentando conectar a Photon...");
    }

    public override void OnConnectedToMaster()
    {
        // Esto se llama cuando la conexión es exitosa
        Debug.Log("✅ Conectado a Photon Cloud!");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("❌ Desconectado de Photon: " + cause);
    }
}
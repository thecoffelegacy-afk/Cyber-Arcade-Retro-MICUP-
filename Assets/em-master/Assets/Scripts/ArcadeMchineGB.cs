using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using GB;

public class ArcadeMchineGB : MonoBehaviourPun
{
    public GameObject gbEmulator;
    public GameObject mainCamera;

    private bool isPlayerInside = false;
    private Loader currentLoader;

    private bool isOccupied = false;

    // 🔥 NUEVO: quién está usando la máquina
    private int ownerActorNumber = -1;

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            TryUseMachine();
        }

        // 🔥 salir del emulador
        if (IsLocalOwner() && Input.GetKeyDown(KeyCode.Escape))
        {
            StopUsing();
        }
    }

    void TryUseMachine()
    {
        if (isOccupied)
        {
            Debug.Log("Máquina ocupada");
            return;
        }

        // 🔥 asignar dueño
        photonView.RPC("SetOccupied", RpcTarget.AllBuffered, true, PhotonNetwork.LocalPlayer.ActorNumber);

        StartUsing();
    }

    void StartUsing()
    {
        if (gbEmulator != null)
        {
            gbEmulator.SetActive(true);

            if (mainCamera != null)
                mainCamera.SetActive(false);

            currentLoader = gbEmulator.GetComponentInChildren<Loader>();

            if (currentLoader != null)
            {
                currentLoader.Load("red");
                currentLoader.isUsing = true;

                Debug.Log("Emulador iniciado");
            }
        }
    }

    void StopUsing()
    {
        if (currentLoader != null)
        {
            currentLoader.isUsing = false;
        }

        if (gbEmulator != null)
            gbEmulator.SetActive(false);

        if (mainCamera != null)
            mainCamera.SetActive(true);

        // 🔥 liberar máquina
        photonView.RPC("SetOccupied", RpcTarget.AllBuffered, false, -1);

        Debug.Log("Emulador liberado");
    }

    bool IsLocalOwner()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber;
    }

    [PunRPC]
    void SetOccupied(bool value, int actorNumber)
    {
        isOccupied = value;
        ownerActorNumber = actorNumber;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = false;
    }

    // 🔥 IMPORTANTE PARA STREAMING
    public bool IsBeingUsedByLocalPlayer()
    {
        return IsLocalOwner();
    }
}
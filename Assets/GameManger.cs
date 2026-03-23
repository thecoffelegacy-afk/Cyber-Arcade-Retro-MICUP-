using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    private bool hasSpawned = false;

    void Start()
    {
        TrySpawn();
    }

    public override void OnJoinedRoom()
    {
        TrySpawn();
    }

    void TrySpawn()
    {
        if (hasSpawned) return;
        if (!PhotonNetwork.InRoom) return;

        hasSpawned = true;

        Debug.Log("🚀 Spawneando player: " + PhotonNetwork.NickName);

        PhotonNetwork.Instantiate(
            "Player",
            Vector3.zero,
            Quaternion.identity
        );
    }
}
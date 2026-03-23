using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Instancia el Player en la posición (0,0,0) para este cliente
        GameObject player = PhotonNetwork.Instantiate(
            "Player",
            Vector3.zero,
            Quaternion.identity
        );


    }
}
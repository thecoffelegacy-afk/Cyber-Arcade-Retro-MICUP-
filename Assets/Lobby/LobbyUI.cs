using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LobbyUI : MonoBehaviourPunCallbacks
{
    public Transform contentPanel; // padre de la lista
    public GameObject serverListItemPrefab; // prefab de sala
    public Button createRoomButton;

    void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        PhotonNetwork.JoinLobby(); // para recibir lista de salas
    }

    void OnCreateRoomClicked()
    {
        string roomName = "Sala_" + Random.Range(1000, 9999);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        Hashtable customProps = new Hashtable();
        customProps["Host"] = PhotonNetwork.NickName;
        roomOptions.CustomRoomProperties = customProps;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "Host" };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform t in contentPanel)
        {
            Destroy(t.gameObject); // limpiar lista anterior
        }

        foreach (RoomInfo room in roomList)
        {
            GameObject item = Instantiate(serverListItemPrefab, contentPanel);
            ServerListItem listItem = item.GetComponent<ServerListItem>();
            listItem.Setup(room.Name, (string)room.CustomProperties["Host"], room.PlayerCount, room.MaxPlayers);
        }
    }
}

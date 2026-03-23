using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ServerListItem : MonoBehaviour
{
    public Text roomNameText;
    public Text hostText;
    public Text playersText;
    public Button joinButton;
    private string roomName;

    public void Setup(string name, string host, int players, int maxPlayers)
    {
        roomName = name;
        roomNameText.text = name;
        hostText.text = host;
        playersText.text = players + "/" + maxPlayers;
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    void OnJoinClicked()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
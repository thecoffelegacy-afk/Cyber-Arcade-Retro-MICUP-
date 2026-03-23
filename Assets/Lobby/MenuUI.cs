using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public Button joinButton;

    void Start()
    {
        // Limita el username a 12 caracteres
        usernameInput.characterLimit = 12;
        joinButton.interactable = false;

        // Listener del botón y del input
        joinButton.onClick.AddListener(OnJoinClicked);
        usernameInput.onValueChanged.AddListener(OnUsernameChanged);

        // Asegura que la escena se sincronice entre todos los jugadores de la sala
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void OnUsernameChanged(string value)
    {
        // Solo activa el botón si hay texto válido
        joinButton.interactable = !string.IsNullOrWhiteSpace(value);
    }

    void OnJoinClicked()
    {
        string username = usernameInput.text.Trim();
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogWarning("Ingresa un username válido!");
            return;
        }

        PhotonNetwork.NickName = username;

        // Conecta a Photon Cloud
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conectando a Photon Cloud...");
    }

    // 🔑 Callback cuando estamos conectados al Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Conectado a Photon Cloud. Entrando a la sala fija...");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 16
        };

        // Une o crea la sala "MainRoom"
        PhotonNetwork.JoinOrCreateRoom("MainRoom", roomOptions, TypedLobby.Default);
    }

    // 🔑 Callback cuando entramos a la sala
    // 🔑 Callback cuando entramos a la sala
    public override void OnJoinedRoom()
    {
        Debug.Log($"✅ Has entrado a la sala: {PhotonNetwork.CurrentRoom.Name} | " +
                  $"Jugadores: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        // 🔍 Debug adicional para revisar el estado de la conexión
        Debug.Log($"🔹 PhotonNetwork.IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
        Debug.Log($"🔹 PhotonNetwork.InRoom: {PhotonNetwork.InRoom}");
        Debug.Log($"🔹 PhotonNetwork.LocalPlayer: {PhotonNetwork.LocalPlayer.NickName}");

        // 🔥 IMPORTANTE: usar Photon para sincronizar la escena, no SceneManager
        PhotonNetwork.LoadLevel("Game");
    }
}

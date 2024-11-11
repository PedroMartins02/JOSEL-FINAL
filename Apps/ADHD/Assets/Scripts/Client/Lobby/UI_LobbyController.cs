using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    //[SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform playerContainer;


    private void Awake()
    {
        // Hide the player Template
        playerSingleTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();

        LobbyManager.Instance.OnJoinedLobby += Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate += Update_OnJoinedLobby;
    }

    private void Update_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        ClearLobby();

        if (playerSingleTemplate != null && playerContainer != null)
        {
            foreach (Player player in lobby.Players)
            {
                if (player != null && MultiplayerManager.Instance.GetPlayerDataFromPlayerId(player.Id).playerId.ToString().Length > 0)
                {
                    Transform playerSingleTransform = Instantiate(playerSingleTemplate, playerContainer);
                    playerSingleTransform.gameObject.SetActive(true);
                    UI_LobbyPlayerSingle lobbyPlayerSingleUI = playerSingleTransform.GetComponent<UI_LobbyPlayerSingle>();

                    // Make the kick button only available to the host/server
                    lobbyPlayerSingleUI.SetKickButtonVisible(
                        NetworkManager.Singleton.IsServer &&
                        player.Id != AuthenticationService.Instance.PlayerId // Don't allow self kick
                    );

                    // Set the ready icon
                    //MP_PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerId(player.Id);
                    //lobbyPlayerSingleUI.SetReadyIconVisible(
                    //    LobbyReady.Instance.IsPlayerReady(playerData.clientId)
                    //);

                    lobbyPlayerSingleUI.UpdatePlayer(player);
                }
            }

            lobbyNameText.text = "Lobby Name:  " + lobby.Name;
            //lobbyCodeText.text = "Lobby Code:  " + lobby.LobbyCode;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

            Show();
        }
    }

    private void ClearLobby()
    {
        if (playerContainer != null)
            foreach (Transform child in playerContainer)
            {
                if (child == playerSingleTemplate) continue;
                Destroy(child.gameObject);
            }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}

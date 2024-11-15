using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI playerOneNameText;
    [SerializeField] private Transform PlayerTwoBox;
    [SerializeField] private TextMeshProUGUI PlayerTwoNameText;
    [SerializeField] private Transform deckSingleTemplate;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private GameObject DeckPrefab;


    private void Awake()
    {
        // Hide the deck Template
        deckSingleTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();

        UpdateDeckList();

        LobbyManager.Instance.OnJoinedLobby += Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate += Update_OnJoinedLobbyUpdate;
    }

    private void Update_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
    }

    private void Update_OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
        UpdateDeckList();
    }


    private void UpdateLobby(Lobby lobby)
    {
        if (deckSingleTemplate != null && deckContainer != null)
        {
            // Update the lobby
            lobbyNameText.text = lobby.Name;
            //lobbyCodeText.text = "Lobby Code:  " + lobby.LobbyCode;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

            // Update the players
            UpdatePlayers(lobby);

            //UpdateDeckList();

            Show();
        }
    }

    private void UpdatePlayers(Lobby lobby)
    {
        // Update the players
        bool firstPlayer = true;
        foreach (Player player in lobby.Players)
        {
            MP_PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerId(player.Id);

            if (player != null && playerData.playerId.ToString().Length > 0)
            {
                if (firstPlayer)
                {
                    playerOneNameText.text = playerData.playerUsername.ToString();
                    firstPlayer = false;
                }
                else
                    PlayerTwoNameText.text = playerData.playerUsername.ToString();
            }
        }
    }

    private void UpdateDeckList()
    {
        ClearDeckList();

        // Update the decks
        PlayerData accountData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = accountData.DeckCollection;

        foreach (DeckData deckData in deckLists)
        {
            var deckInstance = Instantiate(DeckPrefab, deckContainer);
            var deckUI = deckInstance.GetComponent<DeckUI>();
            deckUI.SetDeckData(deckData);
        }
    }

    private void ClearDeckList()
    {
        if (deckContainer != null)
            foreach (Transform child in deckContainer)
            {
                if (child == deckSingleTemplate) continue;
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

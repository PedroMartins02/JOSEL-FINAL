using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyController : MonoBehaviour
{
    [Header("Lobby Overall")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerOneNameText;

    [Header("Player Two")]
    [SerializeField] private TextMeshProUGUI PlayerTwoNameText;
    [SerializeField] private Transform PlayerTwoBox;

    [Header("Rules")]
    [SerializeField] private Transform ruleContainer;
    [SerializeField] private Transform ruleSingleTemplate;

    [Header("Decks")]
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private Transform deckSingleTemplate;

    private void Awake()
    {
        // Hide the deck Template
        deckSingleTemplate.gameObject.SetActive(false);
        ruleSingleTemplate.gameObject.SetActive(false);

        // Hide player two 
        PlayerTwoNameText.gameObject.SetActive(false);
        PlayerTwoBox.gameObject.SetActive(false);
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();

        UpdateDeckList();
        UpdateRules();

        LobbyManager.Instance.OnJoinedLobby += Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate += Update_OnJoinedLobbyUpdate;
    }

    private void Update_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
        UpdateDeckList();
        UpdateRules();
    }

    private void Update_OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
    }


    private void UpdateLobby(Lobby lobby)
    {
        if (deckSingleTemplate != null && deckContainer != null)
        {
            // Update the lobby
            lobbyNameText.text = lobby.Name;

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
                {
                    PlayerTwoNameText.text = playerData.playerUsername.ToString();
                }
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

    private void UpdateRules()
    {
        ClearRulesList();

        // Update the rules
        List<GameModel.GameRule> definedRulesList = MultiplayerManager.Instance.GetLobbyGameRules();

        foreach (GameModel.GameRule gameRule in definedRulesList)
        {
            Transform ruleSingleTransform = Instantiate(ruleSingleTemplate, ruleContainer);
            ruleSingleTransform.gameObject.SetActive(true);

            ruleSingleTransform.GetComponent<UI_LobbyRuleTemplate>().SetRule(gameRule);
        }
    }

    private void ClearRulesList()
    {
        if (ruleContainer != null)
            foreach (Transform child in ruleContainer)
            {
                if (child == ruleSingleTemplate) continue;
                Destroy(child.gameObject);
            }
    }

    public void OnBackBtnClick()
    {
        LobbyManager.Instance.LeaveLobby();
        MultiplayerManager.Instance.LeaveMultiplayerInstance();

        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }

    public void OnKickPlayerClick()
    {
        // Kick player from from Netcode and Lobby
        LobbyManager.Instance.KickPlayers();
        MultiplayerManager.Instance.KickPlayers();
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

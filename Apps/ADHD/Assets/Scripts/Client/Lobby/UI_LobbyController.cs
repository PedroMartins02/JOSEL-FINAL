using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyController : MonoBehaviour
{
    [Header("Lobby Overall")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerOneNameText;

    [Header("Player Two")]
    [SerializeField] private TextMeshProUGUI playerTwoNameText;
    [SerializeField] private Transform playerTwoBox;
    [SerializeField] private Button playerTwoKickButton;

    [Header("Rules")]
    [SerializeField] private Transform ruleContainer;
    [SerializeField] private Transform ruleSingleTemplate;

    [Header("Decks")]
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private Transform deckContainer;

    private Lobby lobby;

    private void Awake()
    {
        // Hide the deck Template
        ruleSingleTemplate.gameObject.SetActive(false);

        // Hide player two 
        playerTwoNameText.gameObject.SetActive(false);
        playerTwoKickButton.gameObject.SetActive(false);
        playerTwoBox.gameObject.SetActive(false);
    }

    private void Start()
    {
        lobby = LobbyManager.Instance.GetLobby();

        UpdateDeckList();
        UpdateRules();

        LobbyManager.Instance.OnJoinedLobby += Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate += Update_OnJoinedLobbyUpdate;
    }

    private void Update()
    {
        if (!MultiplayerManager.Instance.IsServer)
            Debug.Log("rules number: " + MultiplayerManager.Instance.GetLobbyGameRules().Count);
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
        UpdateRules();
    }


    private void UpdateLobby(Lobby lobby)
    {
        if (deckContainer != null)
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
        foreach (Player player in lobby.Players)
        {
            MP_PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerId(player.Id);

            if (player != null && playerData.playerId.ToString().Length > 0)
            {
                if (LobbyManager.Instance.IsLobbyHost(player.Id))
                {
                    playerOneNameText.text = playerData.playerUsername.ToString();
                }
                else
                {
                    // Show Player two if there is any
                    playerTwoBox.gameObject.SetActive(true);

                    playerTwoNameText.gameObject.SetActive(true);
                    playerTwoNameText.text = playerData.playerUsername.ToString();

                    if(LobbyManager.Instance.IsLobbyHost() && MultiplayerManager.Instance.IsServer)
                    {
                        playerTwoKickButton.gameObject.SetActive(true);
                    }
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
            var deckInstance = Instantiate(deckPrefab, deckContainer);
            var deckUI = deckInstance.GetComponent<DeckUI>();
            deckUI.SetDeckData(deckData);
        }
    }

    private void ClearDeckList()
    {
        if (deckContainer != null)
            foreach (Transform child in deckContainer)
            {
                Destroy(child.gameObject);
            }
    }

    private void UpdateRules()
    {
        ClearRulesList();

        List<GameModel.GameRule> lobbyRulesList = MultiplayerManager.Instance.GetLobbyGameRules();

        foreach (GameModel.GameRule gameRule in lobbyRulesList)
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

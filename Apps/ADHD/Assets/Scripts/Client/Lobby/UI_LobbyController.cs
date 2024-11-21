using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyController : MonoBehaviour
{
    [Header("Lobby Overall")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerOneNameText;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image readyIcon;
    [SerializeField] private Sprite notReadyIconSprite;
    [SerializeField] private Sprite readyIconSprite;

    [Header("Player Two")]
    [SerializeField] private TextMeshProUGUI playerTwoNameText;
    [SerializeField] private Transform playerTwoBox;
    [SerializeField] private Button playerTwoKickButton;
    [SerializeField] private Image readyIconP2;
    [SerializeField] private Sprite notReadyIconSpriteP2;
    [SerializeField] private Sprite readyIconSpriteP2;

    [Header("Rules")]
    [SerializeField] private Transform ruleContainer;
    [SerializeField] private Transform ruleSingleTemplate;

    [Header("Decks")]
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private Transform deckContainer;
    //[SerializeField] private Sprite deckSelectedSprite;
    //[SerializeField] private Sprite deckHooverSprite;


    private Lobby lobby;

    private void Awake()
    {
        // Hide the deck Template
        ruleSingleTemplate.gameObject.SetActive(false);

        readyButton.gameObject.SetActive(true);
        readyIcon.GameObject().gameObject.SetActive(true);
        readyIconP2.GameObject().gameObject.SetActive(true);

        // Hide player two 
        playerTwoNameText.gameObject.SetActive(false);
        playerTwoKickButton.gameObject.SetActive(false);
        playerTwoBox.gameObject.SetActive(false);

        // Event for the button here cause why not, since I wanna change the sprite too in the script
        readyButton.onClick.AddListener(() =>
        {
            readyButton.gameObject.SetActive(false);
            LobbySelectReady.Instance.SetPlayerReady();
        });
    }

    private void MultiplayerManager_OnGameRulesListChanged(object sender, System.EventArgs e)
    {
        UpdateRules();
    }

    private void Start()
    {
        lobby = LobbyManager.Instance.GetLobby();

        UpdateDeckList();
        UpdateRules();

        LobbyManager.Instance.OnJoinedLobby += Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate += Update_OnJoinedLobbyUpdate;
        LobbySelectReady.Instance.OnReadyChanged += Update_OnReadyChanged;
        // Event to listen for lobby rules changes and update them
        MultiplayerManager.Instance.OnGameRulesListChanged += MultiplayerManager_OnGameRulesListChanged;
    }

    private void Update_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdateLobby(LobbyManager.Instance.GetLobby());
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

                    // Set the ready icon
                    SetReadyIconVisible(
                        LobbySelectReady.Instance.IsPlayerReady(playerData.clientId)
                    );

                    // Set kick button for the host to be able to kick player 2
                    if (LobbyManager.Instance.IsLobbyHost() && MultiplayerManager.Instance.IsServer)
                    {
                        playerTwoKickButton.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void SetReadyIconVisible(bool isReady)
    {
        readyIcon.gameObject.SetActive(true);
        if (isReady)
        {
            if (MultiplayerManager.Instance.IsServer)
                readyIcon.sprite = readyIconSprite;
            else
                readyIcon.sprite = readyIconSpriteP2;
        } else
        {
            if (MultiplayerManager.Instance.IsServer)
                readyIcon.sprite = notReadyIconSprite;
            else
                readyIcon.sprite = notReadyIconSpriteP2;
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

            // Set the deck data on the UI component attatched to the new game object
            var deckUI = deckInstance.GetComponent<DeckUI>();
            deckUI.SetDeckData(deckData);

            // Setup Button component and event to be able to select deck
            var deckButton = deckInstance.AddComponent<Button>();
            ColorBlock colorBlock = deckButton.colors;
            colorBlock.highlightedColor = Color.red;
            colorBlock.pressedColor = Color.gray;
            deckButton.colors = colorBlock;

            deckButton.onClick.AddListener(() =>
            {
                MultiplayerManager.Instance.SetPlayerDeck(deckData);
            });

            deckInstance.SetActive(true);
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

using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Drawing;
using Color = UnityEngine.Color;

public class UI_LobbyController : MonoBehaviour
{
    [Header("Lobby Overall")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private Button readyButton;

    [Header("Player One")]
    [SerializeField] private TextMeshProUGUI playerOneNameText;
    [SerializeField] private Image readyIconP1;
    [SerializeField] private Sprite notReadyIconSpriteP1;
    [SerializeField] private Sprite readyIconSpriteP1;

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
    [SerializeField] private Shader deckSelectedShader;


    private Lobby lobby;
    private Button previousSelectedBtn;
    private GameObject previousHighlight;

    private void Awake()
    {
        // Hide the deck Template
        ruleSingleTemplate.gameObject.SetActive(false);

        readyButton.gameObject.SetActive(true);
        readyIconP1.gameObject.SetActive(true);

        // Hide player two 
        playerTwoNameText.gameObject.SetActive(true);
        readyIconP2.GameObject().gameObject.SetActive(true);
        playerTwoKickButton.gameObject.SetActive(false);
        playerTwoBox.gameObject.SetActive(false);

        // Event for the button here cause why not, since I wanna change the sprite too in the script
        readyButton.onClick.AddListener(() =>
        {
            LobbySelectReady.Instance.ChangePlayerReady();
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
                    playerTwoNameText.gameObject.SetActive(true);
                    playerTwoNameText.text = playerData.playerUsername.ToString();

                    // Set kick button for the host to be able to kick player 2
                    if (LobbyManager.Instance.IsLobbyHost() && MultiplayerManager.Instance.IsServer)
                    {
                        playerTwoKickButton.gameObject.SetActive(true);
                    }
                }

                // Set the ready icon
                SetReadyIconVisibility(
                    player.Id, LobbySelectReady.Instance.IsPlayerReady(playerData.clientId)
                );
            }
        }

        // Show Player two if there is any
        if(lobby.Players.Count < 2)
            playerTwoBox.gameObject.SetActive(false);
        else
            playerTwoBox.gameObject.SetActive(true);
    }

    public void SetReadyIconVisibility(string playerId, bool isReady)
    {
        if (LobbyManager.Instance.IsLobbyHost(playerId))
        {
            readyIconP1.gameObject.SetActive(true);
            if (isReady)
                readyIconP1.sprite = readyIconSpriteP1;
            else
                readyIconP1.sprite = notReadyIconSpriteP1;
        } 
        else if (LobbyManager.Instance.IsPlayerInLobby(playerId)) 
        {
            readyIconP2.gameObject.SetActive(true);
            if (isReady)
                readyIconP2.sprite = readyIconSpriteP2;
            else
                readyIconP2.sprite = notReadyIconSpriteP2;
        }
    }

    private void UpdateDeckList()
    {
        ClearDeckList();

        // Update the decks
        PlayerData accountData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = accountData.DeckCollection;

        bool isFirst = true;
        foreach (DeckData deckData in deckLists)
        {
            GameObject deckInstance = Instantiate(deckPrefab, deckContainer);

            // Set the deck data on the UI component attatched to the new game object
            DeckUI deckUI = deckInstance.GetComponent<DeckUI>();
            deckUI.SetDeckData(deckData);

            // Setup Button component and event to be able to select deck
            Button deckButton = deckInstance.AddComponent<Button>();
            ColorBlock colorBlock = deckButton.colors;
            colorBlock.highlightedColor = Color.red;
            colorBlock.pressedColor = Color.gray;
            deckButton.colors = colorBlock;

            // Adding highlight game object because WHY IS IT GOOFY
            GameObject highlight = new GameObject("HighlightGO");
            highlight.transform.SetParent(deckInstance.transform, false);

            Image highlightImage = highlight.AddComponent<Image>();

            // Shader for gradient time!
            Material gradientMaterial = new Material(this.deckSelectedShader);
            gradientMaterial.SetColor("_Color", new Color(1f, 0.843f, 0f, 1f));
            gradientMaterial.SetFloat("_Radius", 0.5f);
            highlightImage.material = gradientMaterial;

            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.offsetMin = Vector2.zero;
            highlightRect.offsetMax = Vector2.zero;

            highlightImage.raycastTarget = true;

            highlight.SetActive(false);

            // Set the event for selecting the deck to be used in-game :D
            deckButton.onClick.AddListener(() =>
            {
                DeckData previousDeck = MultiplayerManager.Instance.GetPlayerDeck();

                if (previousDeck == null)
                {
                    MultiplayerManager.Instance.SetPlayerDeck(deckData);
                    deckButton.enabled = false;
                    highlight.SetActive(true);
                    this.previousSelectedBtn = deckButton;
                    this.previousHighlight = highlight;
                }
                if (previousDeck != null && !previousDeck.Equals(deckData))
                {
                    MultiplayerManager.Instance.SetPlayerDeck(deckData);
                    this.previousHighlight.SetActive(false);
                    this.previousSelectedBtn.enabled = true;

                    deckButton.enabled = false;
                    highlight.SetActive(true);
                    this.previousSelectedBtn = deckButton;
                    this.previousHighlight = highlight;
                }
            });

            // Assign the first dick as the selected one
            if (isFirst)
            {
                deckButton.onClick.Invoke();
                isFirst = false;
            }

            deckInstance.SetActive(true);
        }

        if (deckContainer.GetChild(0) != null)
        {
            // Set the size for the deck scroll
            RectTransform contentRect = deckContainer.GetComponent<RectTransform>();
            RectTransform firstChild = deckContainer.GetChild(0).GetComponent<RectTransform>(); ;

            int aproxRows = DivideRoundingUp(deckLists.Count, 3);

            float totalHeight = 350 * aproxRows + 20 + 50 * (aproxRows - 1);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }
    }

    private int DivideRoundingUp(int x, int y)
    {
        int remainder;
        int quotient = Math.DivRem(x, y, out remainder);
        return remainder == 0 ? quotient : quotient + 1;
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

    public async void OnBackBtnClick()
    {

        if (LobbyManager.Instance.IsLobbyHost())
            await LobbyManager.Instance.HostLeaveLobby();
        else
            await LobbyManager.Instance.LeaveLobby();

        MultiplayerManager.Instance.LeaveMultiplayerInstance();

        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }

    public void OnKickPlayerClick()
    {
        // Kick player from from Netcode and Lobby
        LobbyManager.Instance.KickPlayers();
        MultiplayerManager.Instance.KickPlayersFromInstance();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnJoinedLobby -= Update_OnJoinedLobby;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= Update_OnJoinedLobbyUpdate;
        MultiplayerManager.Instance.OnGameRulesListChanged -= MultiplayerManager_OnGameRulesListChanged;
    }
}

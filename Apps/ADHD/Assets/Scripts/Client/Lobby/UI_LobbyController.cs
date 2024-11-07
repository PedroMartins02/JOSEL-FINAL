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

    /**
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform player1Slot;
    [SerializeField] private Transform player2Slot;

    private bool hasReceivedPlayerData = false;

    void Start()
    {
        nameText.text = RelayManager.Singleton.currentLobby.Name;
        AddCallbacks();
        InstantiatePlayerPrefab(true);
        StartCoroutine(CheckLobbyExistenceCoroutine());
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
        RelayManager.Singleton.CloseConnection();
    }

    private void InstantiatePlayerPrefab(bool isMe)
    {
        Transform slot = RelayManager.Singleton.isHostingLobby
                        ? (isMe ? player1Slot : player2Slot)
                        : (isMe ? player2Slot : player1Slot);

        GameObject playerObject = Instantiate(playerPrefab, slot);
        //PlayerUI playerUI = playerObject.GetComponent<PlayerUI>();
        //playerUI.SetPlayerData(reee);
    }

    private void DestroyPlayerPrefab(bool isMe)
    {
        Transform slot = RelayManager.Singleton.isHostingLobby
                        ? (isMe ? player1Slot : player2Slot)
                        : (isMe ? player2Slot : player1Slot);

        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddCallbacks()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped += _ => NavigateBack();
        //OnReadyChange = (playerId, isReady) to update UI
        //OnCountdownStart - to start countown 
        //OnCountdownCancel - to stop countdown
        //OnGameStart - to navigate to game scene
    }

    private void RemoveCallbacks()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped -= _ => NavigateBack();
    }

    IEnumerator CheckLobbyExistenceCoroutine()
    {
        while (!RelayManager.Singleton.isHostingLobby && !hasReceivedPlayerData)
        {
            yield return new WaitForSeconds(2f);

            Task<Lobby> checkLobbyTask = LobbyService.Instance.GetLobbyAsync(RelayManager.Singleton.currentLobby.Id);

            yield return new WaitUntil(() => checkLobbyTask.IsCompleted);

            if (checkLobbyTask.IsFaulted || checkLobbyTask.IsCanceled)
            {
                Debug.LogWarning("Lobby no longer exists (error occurred). Returning to navigation scene.");
                NavigateBack();
                yield break;
            }

            Lobby lobby = checkLobbyTask.Result;
            if (lobby == null)
            {
                Debug.LogWarning("Lobby no longer exists. Returning to navigation scene.");
                NavigateBack();
                yield break;
            }
        }
    }


    private void OnPlayerJoined(ulong clientId)
    {
        hasReceivedPlayerData = true;
        Debug.Log($"Player {clientId} has joined.");
        InstantiatePlayerPrefab(false);
    }

    private void OnPlayerLeft(ulong clientId)
    {
        Debug.Log($"Player {clientId} has left.");
        if (RelayManager.Singleton.isHostingLobby)
        {
            DestroyPlayerPrefab(false);
            return;
        }

        DestroyPlayerPrefab(true);
        NavigateBack();
    }

    public void NavigateBack()
    {
        SceneManager.LoadScene("NavigationScene");
    }

    public void SetReady()
    {
        /*
        ready = !ready
        UpdateUI -> change ready button to cancel and vice versa / update player prefab to display ready state
        SendToServer
         *
    }
    */
}

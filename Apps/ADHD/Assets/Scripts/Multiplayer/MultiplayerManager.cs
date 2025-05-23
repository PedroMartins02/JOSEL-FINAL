using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LobbyManager;

/**
 * This Class serves for handling the network connection (not to confuse with lobby joing/disconnect) and the player's data
 * 
 */
public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public const int MAX_PLAYER_AMOUNT = 2;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    private bool _isMultiplayer = false;
    private List<GameModel.GameRule> lobbyGameRules;
    private DeckData selectedDeckData;
    private NetworkList<MP_PlayerData> playerDataNetworkList;

    // Events for Clients establishing connection to the network 
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    // Event for when the list of Players in the network and their info change
    public event EventHandler OnPlayerDataNetworkListChanged;
    // Event for when the list of rules in the network and their info change
    public event EventHandler OnGameRulesListChanged;

   

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initialize networkList here otherwise error
        playerDataNetworkList = new NetworkList<MP_PlayerData>();

        // Listen for NetworkList changed event
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }


    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<MP_PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        // The event for the host when he needs to approve a connection
        //NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        // Populates the NetworkList with it self's data
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        // If player disconnects, clear his data from the NetworkList
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

        _isMultiplayer = true;

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            MP_PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // This is the player that disconnected, so remove his data
                playerDataNetworkList.RemoveAt(i);
            }
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new MP_PlayerData
        {
            clientId = clientId,
        });

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return;
        }

        // Only add deck if its a quick match
        Lobby lobby = LobbyManager.Instance.GetLobby();

        if (lobby != null && lobby.Data["Type"].Value == LobbyType.QuickMatch.ToString())
        {
            Debug.Log("here host");

            List<DeckData> deckLists = playerData.DeckCollection;
            int deckId = playerData.SelectedDeckId;
            DeckData deck = deckLists[deckId];
            SetPlayerDeck(deck);

            Debug.Log("deck: " + DeckData.SerializeDeckData(deck));
        }
        
        SetPlayerWeatherServerRpc(AccountManager.Singleton.WeatherElement);
        SetPlayerTimeServerRpc(AccountManager.Singleton.TimeElement);
        SetPlayerNameServerRpc(playerData.Name);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        SetPlayerMMRServerRpc(AccountManager.Singleton.GetPlayerData().MMR);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        // Prevent Players from joining if we are not in the LobbyScene
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.Lobby.ToString()
            || SceneManager.GetActiveScene().name != SceneLoader.Scene.NavigationScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        Debug.Log(" Number of Connected Clients: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        // Prevent Players from joining if the lobby is full
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            Debug.Log("here: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }


    public void StartClient()
    {
        // In case user fails to connect, invoke the event to show the reason for disconnecting
        // Otherwise, show the ConnectingUI before entering the lobby
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        _isMultiplayer = true;

        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong obj)
    {
        // To tell the server the username and playerId, using ServerRpc
        var playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return;
        }

        // Only add deck if its a quick match
        Lobby lobby = LobbyManager.Instance.GetLobby();

        if (lobby != null && LobbyManager.Instance.isQuick)
        {
            Debug.Log("here client");

            List<DeckData> deckLists = playerData.DeckCollection;
            int deckId = playerData.SelectedDeckId;
            DeckData deck = deckLists[deckId];
            SetPlayerDeck(deck);

            Debug.Log("deck: " + DeckData.SerializeDeckData(deck));
        }


        SetPlayerWeatherServerRpc(AccountManager.Singleton.WeatherElement);
        SetPlayerTimeServerRpc(AccountManager.Singleton.TimeElement);
        SetPlayerNameServerRpc(playerData.Name);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        SetPlayerMMRServerRpc(AccountManager.Singleton.GetPlayerData().MMR);

        // Request the Lobby Game Rules
        RequestRulesServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void RequestRulesServerRpc(RpcParams rpcParams = default)
    {
        string serializedRules = GameModel.GameRule.SerializeGameRules(this.lobbyGameRules);
        SendRulesClientRpc(serializedRules, rpcParams.Receive.SenderClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SendRulesClientRpc(string serializedRules, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            SetLobbyGameRules(GameModel.GameRule.DeserializeGameRules(serializedRules));
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerWeatherServerRpc(GameModel.Elements playerWeather, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerWeather = playerWeather;

        playerDataNetworkList[playerDataIndex] = playerData;
    }


    [Rpc(SendTo.Server)]
    private void SetPlayerTimeServerRpc(GameModel.Elements playerTime, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerTime = playerTime;

        playerDataNetworkList[playerDataIndex] = playerData;
    }


    [Rpc(SendTo.Server)]
    private void SetPlayerNameServerRpc(string playerUsername, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerUsername = playerUsername;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerIdServerRpc(string playerId, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);

        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerMMRServerRpc(int MMR, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);

        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.MMR = MMR;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public MP_PlayerData GetPlayerDataFromPlayerId(string playerId)
    {
        foreach (MP_PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.playerId == playerId)
                return playerData;
        }
        return default;
    }

    public MP_PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (MP_PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        }
        return -1;
    }

    public MP_PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public bool IsPlayerInstanceHost(ulong clientId)
    {
        if (IsServer && NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            return true;
        }

        return false;
    }

    public bool IsPlayerConnectedClient(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            return true;
        }

        return false;
    }

    /**
     * Method to kick the players from their connection to the host
     */
    public void KickPlayersFromInstance()
    {
        foreach(MP_PlayerData playerData in playerDataNetworkList)
        {
            if (!IsPlayerInstanceHost(playerData.clientId))
            {
                NetworkManager.Singleton.DisconnectClient(playerData.clientId);
                // Kicking the player doesnt trigger the disconnect callback, so manually clean up the NetworkList
                NetworkManager_Server_OnClientDisconnectCallback(playerData.clientId);
            }
        }
    }

    public void LeaveMultiplayerInstance()
    {
        if (IsServer)
        {
            KickPlayersFromInstance();
        }

        NetworkManager.Singleton.Shutdown();
    }

    /**
     * Method to set the game rules
     */
    public void SetLobbyGameRules(List<GameModel.GameRule> rules)
    {
        if (rules != null)
        {
            this.lobbyGameRules = rules;
            OnGameRulesListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /**
     * Method to get the game rules
     */
    public List<GameModel.GameRule> GetLobbyGameRules()
    {
        if (this.lobbyGameRules != null)
            return this.lobbyGameRules;
        else
            return new List<GameModel.GameRule>();
    }

    /**
     * Method to set this player selected deck
     */
    public void SetPlayerDeck(DeckData deckData)
    {
        if (deckData != null)
        {
            this.selectedDeckData = deckData;

            string serializedDeck = DeckData.SerializeDeckData(this.selectedDeckData);
            SetPlayerDeckServerRpc(serializedDeck);

            Debug.Log("Changed deck to: " + deckData.ToString());
        }
    }

    public DeckData GetPlayerDeck()
    {
        return this.selectedDeckData;
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerDeckServerRpc(string serializedDeck, RpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerDeck = serializedDeck;

        playerDataNetworkList[playerDataIndex] = playerData;
    }


    public override void OnDestroy()
    {
        if (_isMultiplayer && NetworkManager.Singleton != null)
        {
            if (IsServer)
            {
                //NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
                NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
            }
            else
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
                NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
            }
        }

        base.OnDestroy();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}

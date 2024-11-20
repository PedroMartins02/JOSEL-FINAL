using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This Class serves for handling the network connection (not to confuse with lobby joing/disconnect) and the player's data
 * 
 */
public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public const int MAX_PLAYER_AMOUNT = 2;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

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
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        // Host populates the NetworkList with the connected player data when he connects
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        // If player disconnects, clear his data from the NetworkList
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

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

        var playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return;
        }

        SetPlayerNameServerRpc(playerData.Name); ;
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        // Prevent Players from joining if we are not in the LobbyScene

        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.Lobby.ToString())
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

        SetPlayerNameServerRpc(playerData.Name);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);

        // Request the Lobby Game Rules
        RequestRulesServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRulesServerRpc(ServerRpcParams rpcParams = default)
    {
        string serializedRules = GameModel.GameRule.SerializeGameRules(this.lobbyGameRules);
        SendRulesClientRpc(serializedRules, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SendRulesClientRpc(string serializedRules, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            SetLobbyGameRules(GameModel.GameRule.DeserializeGameRules(serializedRules));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerUsername, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerUsername = playerUsername;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        MP_PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
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

    private bool IsPlayerInstanceHost(ulong clientId)
    {
        if (IsServer && NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            return true;
        }

        return false;
    }

    /**
     * Method to kick the players from their connection to the host
     */
    public void KickPlayers()
    {
        foreach(MP_PlayerData playerData in playerDataNetworkList)
        {
            if (!IsPlayerInstanceHost(playerData.clientId))
            {
                NetworkManager.Singleton.DisconnectClient(playerData.clientId);
                // Kicking the player doesnt trigger the disconnect callback, so manually clean up the NetworkList
                NetworkManager_Server_OnClientDisconnectCallback(playerData.clientId);

                playerDataNetworkList.Remove(playerData);
            }
        }
    }

    public void LeaveMultiplayerInstance()
    {
        if(NetworkManager.Singleton)
        {
            KickPlayers();
        }

        //playerDataNetworkList.Remove(GetPlayerData());
        playerDataNetworkList = new NetworkList<MP_PlayerData>();

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
            Debug.Log("Changed deck to: " + deckData.ToString());
        }
    }

    public DeckData GetPlayerDeck()
    {
        return this.selectedDeckData;
    }


    public override void OnDestroy()
    {

        base.OnDestroy();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}

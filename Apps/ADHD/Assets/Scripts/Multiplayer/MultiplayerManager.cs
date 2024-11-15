using System;
using System.Collections;
using System.Collections.Generic;
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
    private NetworkList<MP_PlayerData> playerDataNetworkList;

    // Events for Clients establishing connection to the network 
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    // Event for when the list of Players in the network and their info change
    public event EventHandler OnPlayerDataNetworkListChanged;



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
        // The connection approval for the host
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        // Populate the NetworkList with the connected player data whene he connects
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
        // To tell the server the playerId, use ServerRpc
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
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


    /**
     * Method to kick the player from his connection to the host
     */
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        // Kicking the player doesnt trigger the disconnect callback, so manually clean up the NetworkList
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

    /**
     * Method to set the game rules
     */
    public void SetLobbyGameRules(List<GameModel.GameRule> rules)
    {
        if(rules != null)
            this.lobbyGameRules = rules;
    }

    /**
     * Method to get the game rules
     */
    public List<GameModel.GameRule> GetLobbyGameRules()
    {
        return this.lobbyGameRules;
    }
}

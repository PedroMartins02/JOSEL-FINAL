using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Relay;
using UnityEngine.SceneManagement;
using System.Linq;

/**
 * This Class serves for handling everything Lobby related, from Relay to lobby disconnects and joins
 * 
 */
public class LobbyManager : NetworkBehaviour
{
    public enum LobbyType
    {
        CustomMatch, // Game with customized rules
        QuickMatch // Game with normal rules & quick search
    }


    public static LobbyManager Instance { get; private set; }

    private const string KEY_RELAY_CODE = "RelayJoinCode";

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;
    private float lobbyPollTimer = 2;

    // Events for before joining lobby
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnQuickMatchStarted;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickMatchFailed;
    public event EventHandler OnJoinFailed;

    // Events for after joining lobby
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnQuickMatchJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

    // Event for keeping up with the lobby list
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Automatically initialize player into the unity services for now, while not using authentication
        InitializeUnityAuthentication();
    }

    private void LateUpdate()
    {
        HandleLobbyHeartbeat();
        //HandlePeriodicListLobbies(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyPolling();
    }


    public async void InitializeUnityAuthentication()
    {
        // We only want to initialize once, otherwise error
        // So if not already initialized, then initialize
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            // Assign an id for each player even on different builds/same PC
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /**
     * Method for sending heartbeat ping to the lobby service, so that the lobby doesnt become inactive
     */
    private async void HandleLobbyHeartbeat()
    {
        // We only want to run this in the host
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                // Ping for the lobby
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    // For Updating the lobby UI preriodically with the players every lobbyPoolTimer
    private async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 3f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby())
                {
                    joinedLobby = null;
                }
            }
        }
    }

    public void HandleLobbyListRefresh()
    {
        if (joinedLobby == null && UnityServices.State == ServicesInitializationState.Initialized
            && AuthenticationService.Instance.IsSignedIn
            && SceneManager.GetActiveScene().name == SceneLoader.Scene.NavigationScene.ToString())
        {
            // Discard, no need to await because we are using the event to transfert the data
            _ = ListLobbiesOfType(LobbyType.CustomMatch);
        }
    }

    private async Task<List<Lobby>> ListLobbiesOfType(LobbyType type)
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                // We'll list only lobbies with available slots (more than 0 slots)
                // Count = 6,
                Filters = new List<QueryFilter> {
                  new QueryFilter(
                      field: QueryFilter.FieldOptions.AvailableSlots,
                      value: "0",
                      op: QueryFilter.OpOptions.GT),

                  new QueryFilter(
                      field: QueryFilter.FieldOptions.S1,
                      value: type.ToString(),
                      op: QueryFilter.OpOptions.EQ)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
            return queryResponse.Results;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }


    /**
     *  Method for creating the relay in order to make p2p communication in this multiplayer setting
     * 
     */
    private async Task<string> RelayAllocation()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MAX_PLAYER_AMOUNT - 1); // -1 which is the host
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    /**
     *  Method for joining a relay allocation via a join code.
     * 
     */
    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    /**
     * Method for creating and hosting a game Lobby
     */
    public async void CreateLobby(string lobbyName, bool isPrivate, LobbyType lobbyType, List<GameModel.GameRule> gameRules)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            // Keep created lobby as the joined Lobby
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                // Data options for the lobby, which includes the lobby type and the game rules as a string
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { "Type", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public, 
                        value: lobbyType.ToString(),
                        index: DataObject.IndexOptions.S1) 
                    }
                }
            });

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

            // Relay for p2p multiplayer
            string relayJoinCode = await RelayAllocation();

            // Other players need the code created by Relay to join session
            // Because of this, we keep it stored in the lobby data
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            // Set the Game Rules in the host multiplayer instance
            MultiplayerManager.Instance.SetLobbyGameRules(gameRules);

            // Start Hosting and go to LobbyScene, where the main lobby will be
            MultiplayerManager.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.Lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    /**
     * Method for quick matching
     * It joins a Quick Match lobby if any is available, or creates one if not
     */
    public async void QuickMatchLobby()
    {
        OnQuickMatchStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            // Firstly, we try to join an available Quick Match lobby
            List<Lobby> lobbyList = await ListLobbiesOfType(LobbyType.QuickMatch);

            // If there is a Quick Match lobby available, join first available, otherwise create one
            if(lobbyList.Count > 0)
            {
                Lobby firstLobby = lobbyList.First<Lobby>();
                if (firstLobby != null)
                {
                    JoinLobbyById(firstLobby.Id);
                }
            } 
            else
            {
                CreateLobby(
                    "QuickMatchLobby",
                    true,
                    LobbyManager.LobbyType.QuickMatch,
                    GameModel.GameRule.GetDefaultRules()
                );
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickMatchFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    /**
     * Method for joining a lobby by clicking on it
     */
    public async void JoinLobbyById(string lobbyID)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);

            // Join a lobby with Relay
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

            // Enter the game lobby as a client
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    /**
     * Method for returning true if THIS is the host
     */
    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public bool IsLobbyHost(string playerId)
    {
        return joinedLobby != null && joinedLobby.HostId == playerId;
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                
                if (IsLobbyHost())
                {
                    KickPlayers();
                }

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayers()
    {
        if (IsLobbyHost())
        {
            try
            {
                // Get the other players
                List<Player> playerList = joinedLobby.Players;

                foreach (Player player in playerList)
                {
                    if(!player.Id.Equals(AuthenticationService.Instance.PlayerId))
                        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, player.Id);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public override void OnDestroy()
    {
        LeaveLobby();

        // Always invoke the base 
        base.OnDestroy();
    }

    void OnApplicationQuit()
    {
        LeaveLobby();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}

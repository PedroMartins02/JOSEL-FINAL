using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

public class RelayManager : MonoBehaviour
{
    public bool isHostingLobby = false;
    public Lobby currentLobby;
    private static RelayManager _singleton;

    public static RelayManager Singleton
    {
        get => _singleton;
        private set
        {
            if (null == _singleton) 
                _singleton = value;
            else if (value != _singleton)
            {
                Destroy(value);
            }
        }
    }

    async void Start()
    {
        await UnityServices.InitializeAsync(); 
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task<string> HostRelay(int maxPlayers)
    {
        if (maxPlayers < 1) { return null; }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        return relayJoinCode;
    }

    private async Task<bool> CreateLobby(string name, string code, int maxPlayers, string lobbyType)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
                { "Type", new DataObject(DataObject.VisibilityOptions.Public, lobbyType) },
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, code) },
            }
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, options);
        currentLobby = lobby;
        Debug.Log("Lobby created with ID: " + lobby.Id);
        isHostingLobby = true;
        StartCoroutine(LobbyHeartbeatCoroutine());
        return lobby != null;
    }

    private IEnumerator LobbyHeartbeatCoroutine()
    {
        while (isHostingLobby)
        {
            yield return new WaitForSeconds(30f); // Ping every 30 seconds

            if (currentLobby == null)
            {
                continue;
            }

            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                Debug.Log("Lobby heartbeat sent for Lobby ID: " + currentLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to send lobby heartbeat: " + e.Message);
                // Handle failure (e.g., lobby might no longer exist)
            }
        }
    }

    private async void CloseLobby()
    {
        isHostingLobby = false; // Stop the coroutine
        if (currentLobby == null) { return; }

        await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
        Debug.Log("Lobby deleted: " + currentLobby.Id);
        currentLobby = null;
    }

    private async Task<IEnumerable<Lobby>> SearchLobbiesOfType(string type)
    {
        QueryResponse response = null;
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            response = await LobbyService.Instance.QueryLobbiesAsync(options);
        } 
        catch (Exception ex)
        {
            Debug.Log($"Failed to find Lobbies: {ex.Message}");
        }

        if (response == null || !response.Results.Any()) { return Enumerable.Empty<Lobby>(); }

        return response.Results.Where(lobby => lobby.Data["Type"].Value.Equals(type));
    }

    public async Task<bool> CreateRoom(string lobbyName, string type)
    {
        int maxPlayers = 2;
        string joinCode = await HostRelay(maxPlayers);
        string lobbyType = type;
        return await CreateLobby(lobbyName, joinCode, maxPlayers, lobbyType);
    }

    public async void JoinQueue()
    {
        string lobbyType = "Matchmaking";
        string lobbyName = "Matchmaking";
        var lobbies = await SearchLobbiesOfType(lobbyType);
        if (lobbies.Any())
        {
            await JoinRoom(lobbies.First().Data["RelayJoinCode"].Value);
            return;
        }

        await CreateRoom(lobbyName, lobbyType);
    }

    public async Task<IEnumerable<Lobby>> ListRooms()
    {
        return await SearchLobbiesOfType("Custom");
    }

    public async Task<bool> JoinRoom(string joinCode)
    {
        try {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            Debug.Log($"Joined Lobby: {relayServerData}");
            return true;
        } catch (Exception ex)
        {
            Debug.Log($"Failed to join Lobby: {ex.Message}");
            return false;
        }
    }

    public bool CloseConnection()
    {
        NetworkManager.Singleton.Shutdown();
        CloseLobby();
        return true;
    }
}

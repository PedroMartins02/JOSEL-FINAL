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
    [SerializeField] Button hostButton;
    [SerializeField] Button listButton;
    [SerializeField] Button queueButton;

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomListContainer;

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
        // =========== this should be somewhere else (i think), just not sure where : )
        await UnityServices.InitializeAsync(); 
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        // =========== this should be somewhere else (i think), just not sure where : )

        hostButton.onClick.AddListener(() => CreateRoom("Custom"));
        listButton.onClick.AddListener(ListRooms);
        queueButton.onClick.AddListener(JoinQueue);
    }

    private void Awake()
    {
        Singleton = this;
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

    private async void CreateLobby(string name, string code, int maxPlayers, string lobbyType)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
                { "Type", new DataObject(DataObject.VisibilityOptions.Public, lobbyType) },
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, code) },
            }
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, options);
        Debug.Log("Lobby created with ID: " + lobby.Id);
    }

    private async Task<IEnumerable<Lobby>> SearchLobbiesOfType(string type)
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);

        if (!response.Results.Any()) { return Enumerable.Empty<Lobby>(); }

        return response.Results.Where(lobby => lobby.Data["Type"].Value.Equals(type));
    }

    async void CreateRoom(string type)
    {
        int maxPlayers = 2;
        string joinCode = await HostRelay(maxPlayers);
        string lobbyName = "Placeholder";
        string lobbyType = type;
        CreateLobby(lobbyName, joinCode, maxPlayers, lobbyType);
    }

    public async void JoinQueue()
    {
        string lobbyType = "Matchmaking";
        var lobbies = await SearchLobbiesOfType(lobbyType);
        if (lobbies.Any())
        {
            JoinRoom(lobbies.First().Data["RelayJoinCode"].Value);
            return;
        }

        CreateRoom(lobbyType);
    }

    private void ClearRoomsList()
    {
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    async void ListRooms()
    {
        var lobbies = await SearchLobbiesOfType("Custom");
        if (!lobbies.Any()) { return; }

        ClearRoomsList();

        foreach (var lobby in lobbies)
        {
            // Instantiate the room prefab and set its parent to the container
            GameObject roomUI = Instantiate(roomPrefab, roomListContainer);

            // Find the UI components inside the prefab
            TMP_Text roomNameText = roomUI.transform.Find("RoomName").GetComponent<TMP_Text>();
            Button joinButton = roomUI.transform.Find("JoinButton").GetComponent<Button>();

            // Set the room data
            roomNameText.text = $"Room Name: {lobby.Name}";

            // Set up the join button functionality
            string relayJoinCode = lobby.Data["RelayJoinCode"].Value;
            joinButton.onClick.AddListener(() => JoinRoom(relayJoinCode));

            Debug.Log($"Lobby Name: {lobby.Name}, Players: {lobby.Players.Count}/{lobby.MaxPlayers}");
        }
    }

    async void JoinRoom(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = new RelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        Debug.Log($"Joined Lobby: {relayServerData}");
    }
}

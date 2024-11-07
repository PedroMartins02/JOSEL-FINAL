using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ServerListController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private Transform lobbyContainer;

    private void Start()
    {
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
        RefreshLobbyList();
    }

    public void RefreshLobbyList()
    {
        LobbyManager.Instance.HandleLobbyListRefresh();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        // Clear the already placed lobbies
        foreach (Transform child in lobbyContainer)
        {
            Destroy(child.gameObject);
        }

        // Instanciate the new found ones
        foreach (Lobby lobby in lobbyList)
        {
            GameObject lobbyItem = Instantiate(lobbyPrefab, lobbyContainer);
            UI_Room roomUI = lobbyItem.GetComponent<UI_Room>();
            roomUI.SetLobbyData(lobby);
        }
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    /**
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private Transform contentParent;

    void Start()
    {
        RefreshServerList();
    }

    private void RefreshServerList()
    {
        ClearLobbies();
        FetchServersAsync();
    }

    private void ClearLobbies()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    private async void FetchServersAsync()
    {
        var serverList = await RelayManager.Singleton.ListRooms();

        if (!serverList.Any()) { return; } //throw error

        foreach (var server in serverList)
        {
            Debug.Log($"Lobby Name: {server.Name}, Players: {server.Players.Count}/{server.MaxPlayers}");
        }


        foreach (var server in serverList)
        {
            GameObject lobbyItem = Instantiate(lobbyPrefab, contentParent);
            RoomUI roomUI = lobbyItem.GetComponent<RoomUI>();
            roomUI.SetLobbyData(server);
        }
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerListManager : MonoBehaviour
{
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Players: {NetworkManager.Singleton.ConnectedClientsList}");

        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
        RelayManager.Singleton.CloseConnection();
    }

    private void OnPlayerJoined(ulong clientId)
    {
        Debug.Log($"Player {clientId} has joined.");

        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log($"Current player count: {playerCount}");
    }

    private void OnPlayerLeft(ulong clientId)
    {
        Debug.Log($"Player {clientId} has left.");

        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log($"Current player count: {playerCount}");
    }

    public void NavigateBack()
    {
        NavigateToNavigationScene();
    }

    private void NavigateToNavigationScene()
    {
        SceneManager.LoadScene("NavigationScene");
    }
}

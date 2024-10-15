using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button readyButton;

    void Start()
    {
        nameText.text = RelayManager.Singleton.currentLobby.Name;
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped -= _ => NavigateBack();
        RelayManager.Singleton.CloseConnection();
    }

    private void OnPlayerJoined(ulong clientId)
    {
        Debug.Log($"Player {clientId} has joined.");
    }

    private void OnPlayerLeft(ulong clientId)
    {
        Debug.Log($"Player {clientId} has left.");
        if (RelayManager.Singleton.isHostingLobby)
        {
            //handle client left
            return;
        }

        NavigateToNavigationScene();
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

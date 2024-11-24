using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UI_LobbyHostDisconnect : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (!MultiplayerManager.Instance.IsServer && this.gameObject != null)
        {
            // Server is shutting down
            Show();
            LobbyManager.Instance.ClearJoinedLobby();
        }
    }

    public void LeaveToMainMenu()
    {
        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}

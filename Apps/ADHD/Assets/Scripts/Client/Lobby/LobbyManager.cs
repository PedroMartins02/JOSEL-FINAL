using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button readyButton;

    private bool hasReceivedPlayerData = false;

    void Start()
    {
        nameText.text = RelayManager.Singleton.currentLobby.Name;
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped += _ => NavigateBack();
        StartCoroutine(CheckLobbyExistenceCoroutine());
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped -= _ => NavigateBack();
        RelayManager.Singleton.CloseConnection();
    }

    IEnumerator CheckLobbyExistenceCoroutine()
    {
        while (!RelayManager.Singleton.isHostingLobby && !hasReceivedPlayerData)
        {
            yield return new WaitForSeconds(2f);

            Task<Lobby> checkLobbyTask = LobbyService.Instance.GetLobbyAsync(RelayManager.Singleton.currentLobby.Id);

            yield return new WaitUntil(() => checkLobbyTask.IsCompleted);

            if (checkLobbyTask.IsFaulted || checkLobbyTask.IsCanceled)
            {
                Debug.LogWarning("Lobby no longer exists (error occurred). Returning to navigation scene.");
                NavigateBack();
                yield break;
            }

            Lobby lobby = checkLobbyTask.Result;
            if (lobby == null)
            {
                Debug.LogWarning("Lobby no longer exists. Returning to navigation scene.");
                NavigateBack();
                yield break;
            }
        }
    }


    private void OnPlayerJoined(ulong clientId)
    {
        hasReceivedPlayerData = true;
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

        NavigateBack();
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

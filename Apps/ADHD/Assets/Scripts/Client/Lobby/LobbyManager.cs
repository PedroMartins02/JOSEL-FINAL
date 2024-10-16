using GameModel;
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
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform player1Slot;
    [SerializeField] private Transform player2Slot;

    private bool hasReceivedPlayerData = false;

    void Start()
    {
        nameText.text = RelayManager.Singleton.currentLobby.Name;
        AddCallbacks();
        InstantiatePlayerPrefab(true);
        StartCoroutine(CheckLobbyExistenceCoroutine());
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
        RelayManager.Singleton.CloseConnection();
    }

    private void InstantiatePlayerPrefab(bool isMe)
    {
        Transform slot = RelayManager.Singleton.isHostingLobby
                        ? (isMe ? player1Slot : player2Slot)
                        : (isMe ? player2Slot : player1Slot);

        GameObject playerObject = Instantiate(playerPrefab, slot);
        //PlayerUI playerUI = playerObject.GetComponent<PlayerUI>();
        //playerUI.SetPlayerData(reee);
    }

    private void DestroyPlayerPrefab(bool isMe)
    {
        Transform slot = RelayManager.Singleton.isHostingLobby
                        ? (isMe ? player1Slot : player2Slot)
                        : (isMe ? player2Slot : player1Slot);

        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddCallbacks()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped += _ => NavigateBack();
    }

    private void RemoveCallbacks()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerLeft;
        NetworkManager.Singleton.OnServerStopped -= _ => NavigateBack();
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
        InstantiatePlayerPrefab(false);
    }

    private void OnPlayerLeft(ulong clientId)
    {
        Debug.Log($"Player {clientId} has left.");
        if (RelayManager.Singleton.isHostingLobby)
        {
            DestroyPlayerPrefab(false);
            return;
        }

        DestroyPlayerPrefab(true);
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

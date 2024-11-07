using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Room : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;

    private Lobby lobby;

    public void SetLobbyData(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }

    public void JoinRoom()
    {
        if (lobby == null)
        {
            return;
        } 
        LobbyManager.Instance.JoinLobbyById(lobby.Id);
    }

    /**
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button joinButton;

    private Lobby lobby;

    public void SetLobbyData(Lobby lobby)
    {
        this.lobby = lobby;
        nameText.text = lobby.Name;
    }

    public async void JoinRoom()
    {
        if (lobby == null)
        {
            return;
        }
        Debug.Log("Start Loading");
        if (await RelayManager.Singleton.JoinRoom(lobby))
        {
            NavigateToLobby();
        }
        Debug.Log("Stop Loading");
    }

    private void NavigateToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    */
}

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
    private System.Action startLoading;

    public void SetLobbyData(Lobby lobby, System.Action startLoading)
    {
        this.startLoading = startLoading;
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }

    public void JoinRoom()
    {
        if (lobby == null)
        {
            return;
        }
        startLoading();
        LobbyManager.Instance.JoinLobbyById(lobby.Id);
    }
}

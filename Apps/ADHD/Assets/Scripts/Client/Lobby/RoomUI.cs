using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button joinButton;

    public void SetLobbyData(Lobby lobby)
    {
        nameText.text = lobby.Name;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => RelayManager.Singleton.JoinRoom(lobby.Data["RelayJoinCode"].Value));
    }
}

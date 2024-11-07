using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class UI_LobbyPlayerSingle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private Button kickButton;

    private MP_PlayerData playerData;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            // Kick player from from Netcode and Lobby
            LobbyManager.Instance.KickPlayer(playerData.playerId.ToString());
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }


    public void UpdatePlayer(Player player)
    {
        this.playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerId(player.Id);

        playerNameText.text = playerData.playerUsername.ToString();
    }

    public void SetKickButtonVisible(bool visible)
    {
        kickButton.gameObject.SetActive(visible);
    }

    public void SetReadyIconVisible(bool visible)
    {
        readyIcon.gameObject.SetActive(visible);
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
    }
}

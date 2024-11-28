using Game.Logic;
using GameModel;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameActionController : NetworkBehaviour
{
    [Header("This Player UI")]
    [SerializeField] private HandCardHolder myHand;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image mythVisual;

    [Header("Opponent UI")]
    [SerializeField] private HandCardHolder opponentHand;
    [SerializeField] private TextMeshProUGUI oppHealthText;
    [SerializeField] private Image oppHealthImage;
    [SerializeField] private Image oppMythVisual;

    private void Start()
    {
        GameplayManager.Instance.OnCurrentGameStateChanged += GameplayManager_OnStateChange;
        
        healthImage.fillAmount = 1;
        oppHealthImage.fillAmount = 1;
    }

    public override void OnNetworkSpawn()
    {
        GameplayManager.Instance.OnCurrentGameStateChanged += GameplayManager_OnStateChange; ;

        base.OnNetworkSpawn();
    }

    private void GameplayManager_OnStateChange(object sender, GameplayManager.GameStateEventArgs e)
    {
        Debug.Log("WRRAAAAA final");
        if (IsServer && e.gameState.Equals(GameplayManager.GameState.Playing))
        {
            GameInitializationServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameInitializationServerRpc(ServerRpcParams rpcParams = default)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            UpdateHealthbarClientRpc(clientId);
            UpdateMythClientRpc(clientId);
        }
    }

    [ClientRpc]
    public void UpdateHealthbarClientRpc(ulong clientId)
    {
        GameModel.Player player = PlayerManager.Instance.GetPlayerByClientId(clientId);

        if (player != null)
        {
            if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
            {
                healthImage.fillAmount = player.CurrentHealth / player.playerData.Health;
                healthText.text = player.playerData.Health.ToString();
            }
            else
            {
                oppHealthImage.fillAmount = player.CurrentHealth / player.playerData.Health;
                oppHealthText.text = player.playerData.Health.ToString();
            }
        }
    }

    [ClientRpc]
    public void UpdateMythClientRpc(ulong clientId)
    {
        GameModel.Player player = PlayerManager.Instance.GetPlayerByClientId(clientId);
        CardSO mythSO = CardDatabase.Singleton.GetCardSoOfId(player.MythCard.Data.Id);

        if (player != null && mythSO != null)
        {
            if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
            {
                mythVisual.sprite = mythSO.Art;
            }
            else
            {
                oppMythVisual.sprite = mythSO.Art;
            }
        }
    }

}

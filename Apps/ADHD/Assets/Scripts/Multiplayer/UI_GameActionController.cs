using Game.Logic;
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

    [Header("Opponent UI")]
    [SerializeField] private HandCardHolder opponentHand;
    [SerializeField] private TextMeshProUGUI oppHealthText;
    [SerializeField] private Image oppHealthImage;

    private void Start()
    {
        GameplayManager.Instance.OnGameStart += GameplayManager_OnGameStart;

        healthImage.fillAmount = 1;
        oppHealthImage.fillAmount = 1;
    }

    private void GameplayManager_OnGameStart(object sender, System.EventArgs e)
    {
        if(IsServer)
            GameInitializationServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameInitializationServerRpc(ServerRpcParams rpcParams = default)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            UpdateHealthbarClientRpc(clientId);
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

}

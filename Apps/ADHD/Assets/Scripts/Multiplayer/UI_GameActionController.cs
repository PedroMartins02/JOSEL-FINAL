using Game.Data;
using Game.Logic;
using Game.Multiplayer;
using GameCore.Events;
using GameModel;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameActionController : NetworkBehaviour
{
    [Header("This Player UI")]
    [SerializeField] private HandCardHolder myHand;
    [SerializeField] private BoardCardHolder myBoard;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image mythVisual;

    [Header("Opponent UI")]
    [SerializeField] private HandCardHolder opponentHand;
    [SerializeField] private BoardCardHolder opponentBoard;
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
        GameplayManager.Instance.OnCurrentGameStateChanged += GameplayManager_OnStateChange;

        base.OnNetworkSpawn();

        EventManager.Subscribe(GameEventsEnum.CardDrawn, OnCardDrawnEvent);
        EventManager.Subscribe(GameEventsEnum.CardPlayed, OnCardPlayedEvent);
    }

    private void GameplayManager_OnStateChange(object sender, GameplayManager.GameStateEventArgs e)
    {
        if (IsServer && e.gameState.Equals(GameplayManager.GameState.Playing))
        {
            GameInitializationServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void GameInitializationServerRpc(RpcParams rpcParams = default)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameModel.Player player = PlayerManager.Instance.GetPlayerByClientId(clientId);

            if (player == null)
                continue;

            UpdateHealthbarClientRpc(clientId, player.CurrentHealth, player.playerData.Health);

            UpdateMythClientRpc(clientId, player.playerData.MythCard.Data.Id);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateHealthbarClientRpc(ulong clientId, int playerCurrentHealth, int playerMaxHealth)
    {
        if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            healthImage.fillAmount = playerCurrentHealth / playerMaxHealth;
            healthText.text = playerMaxHealth.ToString();
        }
        else
        {
            oppHealthImage.fillAmount = playerCurrentHealth / playerMaxHealth;
            oppHealthText.text = playerMaxHealth.ToString();
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateMythClientRpc(ulong clientId, FixedString64Bytes mythSOId)
    {
        CardSO mythSO = CardDatabase.Singleton.GetCardSoOfId(mythSOId.ToString());

        if (mythSO != null)
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

    public void OnCardDrawnEvent(object args) 
    {
        if (args.GetType() != typeof(CardDrawnEventArgs))
            return;

        CardDrawnEventArgs cardDrawnArgs = (CardDrawnEventArgs)args;

        if (NetworkManager.Singleton.LocalClientId.Equals(cardDrawnArgs.PlayerID))
        {
            myHand.SpawnCard(cardDrawnArgs.CardData, cardDrawnArgs.PlayerID);
        }
        else
        {
            opponentHand.SpawnCard(cardDrawnArgs.CardData, cardDrawnArgs.PlayerID);
        }

        ClientCardManager.Instance.RegisterCardSnapshotRpc(cardDrawnArgs.CardData);
    }

    public void OnCardPlayedEvent(object args)
    {
        if (args.GetType() != typeof(CardPlayedEventArgs))
            return;

        CardPlayedEventArgs cardPlayedArgs = (CardPlayedEventArgs)args;

        GameCard playedCard;

        if (NetworkManager.Singleton.LocalClientId.Equals(cardPlayedArgs.PlayerID))
        {
            playedCard = myHand.cards.Find(card => card.GameID == cardPlayedArgs.CardGameID);

            if (playedCard != null)
                playedCard.PlayCardOnBoard(myBoard.gameObject);
        }
        else
        {
            playedCard = opponentHand.cards.Find(card => card.GameID == cardPlayedArgs.CardGameID);

            if (playedCard != null)
                playedCard.PlayCardOnBoard(opponentBoard.gameObject);
        }

        if (IsServer && playedCard != null)
        {
            ClientCardManager.Instance.UpdateCardSnapshotRpc(cardPlayedArgs.CardGameID);
        }
    }
}

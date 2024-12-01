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
    [SerializeField] private TextMeshProUGUI healthLabel;
    [SerializeField] private TextMeshProUGUI blessingsLabel;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image mythVisual;

    [Header("Opponent UI")]
    [SerializeField] private HandCardHolder opponentHand;
    [SerializeField] private BoardCardHolder opponentBoard;
    [SerializeField] private TextMeshProUGUI oppHealthLabel;
    [SerializeField] private TextMeshProUGUI oppBlessingsLabel;
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

        EventManager.Subscribe(GameEventsEnum.PlayerInfoChanged, UpdatePlayerInfo);
        EventManager.Subscribe(GameEventsEnum.CardDrawn, OnCardDrawnEvent);
        EventManager.Subscribe(GameEventsEnum.CardPlayed, OnCardPlayedEvent);
        EventManager.Subscribe(GameEventsEnum.CardAttacked, OnCardAttackedEvent);
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

            UpdatePlayerMythRpc(clientId, player.playerData.MythCard.Data.Id);
            UpdatePlayerHealthbarRpc(clientId, player.CurrentHealth, player.playerData.Health);
            UpdatePlayerBlessingsRpc(clientId, player.CurrentBlessings, player.CurrentMaxBlessings);
        }
    }

    public void UpdatePlayerInfo(object _)
    {
        UpdatePlayerInfoRpc();
    }

    [Rpc(SendTo.Server)]
    public void UpdatePlayerInfoRpc(RpcParams rpcParams = default)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameModel.Player player = PlayerManager.Instance.GetPlayerByClientId(clientId);

            if (player == null)
                continue;

            UpdatePlayerHealthbarRpc(clientId, player.CurrentHealth, player.playerData.Health);
            UpdatePlayerBlessingsRpc(clientId, player.CurrentBlessings, player.CurrentMaxBlessings);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePlayerHealthbarRpc(ulong clientId, int playerCurrentHealth, int playerMaxHealth)
    {
        if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            healthImage.fillAmount = playerCurrentHealth / playerMaxHealth;
            healthLabel.text = playerMaxHealth.ToString();
        }
        else
        {
            oppHealthImage.fillAmount = playerCurrentHealth / playerMaxHealth;
            oppHealthLabel.text = playerMaxHealth.ToString();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePlayerBlessingsRpc(ulong clientId, int CurrentBlessings, int CurrentMaxBlessings)
    {
        string blessingsText = $"{CurrentBlessings}/{CurrentMaxBlessings}";

        if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            blessingsLabel.text = blessingsText;
        }
        else
        {
            oppBlessingsLabel.text = blessingsText;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePlayerMythRpc(ulong clientId, FixedString64Bytes mythSOId)
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

    public void OnCardAttackedEvent(object args)
    {
        if (args.GetType() != typeof(CardAttackedEventArgs))
            return;

        CardAttackedEventArgs cardAttackedArgs = (CardAttackedEventArgs)args;

        GameCard attackingCard;
        GameCard targetCard;

        if (NetworkManager.Singleton.LocalClientId.Equals(cardAttackedArgs.PlayerID))
        {
            attackingCard = myBoard.cards.Find(card => card.GameID == cardAttackedArgs.AttackingCardGameID);
            targetCard = opponentBoard.cards.Find(card => card.GameID == cardAttackedArgs.TargetCardGameID);
        }
        else
        {
            attackingCard = opponentBoard.cards.Find(card => card.GameID == cardAttackedArgs.AttackingCardGameID);
            targetCard = myBoard.cards.Find(card => card.GameID == cardAttackedArgs.TargetCardGameID);
        }

        if (attackingCard == null || targetCard == null) return;

        // TODO: Do stuff with the cards, animations and such

        if (IsServer)
        {
            ClientCardManager.Instance.UpdateCardSnapshotRpc(cardAttackedArgs.AttackingCardGameID);
            ClientCardManager.Instance.UpdateCardSnapshotRpc(cardAttackedArgs.TargetCardGameID);
        }
    }
}

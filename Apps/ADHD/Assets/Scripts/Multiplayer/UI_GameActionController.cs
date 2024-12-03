using Game.Data;
using Game.Logic;
using Game.Multiplayer;
using GameCore.Events;
using GameModel;
using System.Collections;
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
    [SerializeField] private Image blessingsImage;
    [SerializeField] private Image mythVisual;
    [SerializeField] private Transform myDiscardPile;

    [Header("Opponent UI")]
    [SerializeField] private HandCardHolder opponentHand;
    [SerializeField] private BoardCardHolder opponentBoard;
    [SerializeField] private TextMeshProUGUI oppHealthLabel;
    [SerializeField] private TextMeshProUGUI oppBlessingsLabel;
    [SerializeField] private Image oppHealthImage;
    [SerializeField] private Image oppBlessingsImage;
    [SerializeField] private Image oppMythVisual;
    [SerializeField] private Transform oppDiscardPile;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawCardAudio;
    [SerializeField] private AudioClip playCardAudio;
    [SerializeField] private AudioClip attackCardAudio;
    [SerializeField] private AudioClip destroyCardAudio;
    [SerializeField] private AudioClip attackMythAudio;

    private void Start()
    {
        GameplayManager.Instance.OnCurrentGameStateChanged += GameplayManager_OnStateChange;

        blessingsImage.fillAmount = 1;
        oppBlessingsImage.fillAmount = 1;
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
        EventManager.Subscribe(GameEventsEnum.MythDamaged, OnMythAttackedEvent);
        EventManager.Subscribe(GameEventsEnum.CardDied, OnCardDiedEvent);
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
            healthImage.fillAmount = (float)playerCurrentHealth / (float)playerMaxHealth;
            healthLabel.text = $"{playerCurrentHealth}/{playerMaxHealth}";
        }
        else
        {
            oppHealthImage.fillAmount = (float)playerCurrentHealth / (float)playerMaxHealth;
            oppHealthLabel.text = $"{playerCurrentHealth}/{playerMaxHealth}";
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePlayerBlessingsRpc(ulong clientId, int CurrentBlessings, int CurrentMaxBlessings)
    {
        string blessingsText = $"{CurrentBlessings}/{CurrentMaxBlessings}";

        if (NetworkManager.Singleton.LocalClientId.Equals(clientId))
        {
            blessingsImage.fillAmount = (float)CurrentBlessings / (float)CurrentMaxBlessings;
            blessingsLabel.text = blessingsText;
        }
        else
        {
            oppBlessingsImage.fillAmount = (float)CurrentBlessings / (float)CurrentMaxBlessings;
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

        audioSource.clip = drawCardAudio;
        audioSource.Play();

        CardDrawnEventArgs cardDrawnArgs = (CardDrawnEventArgs)args;

        if (NetworkManager.Singleton.LocalClientId.Equals(cardDrawnArgs.PlayerID))
        {
            myHand.SpawnCard(cardDrawnArgs.CardData, cardDrawnArgs.PlayerID);
        }
        else
        {
            opponentHand.SpawnCard(cardDrawnArgs.CardData, cardDrawnArgs.PlayerID);
        }

        ClientCardManager.Instance.RegisterCardSnapshotRpc(cardDrawnArgs.CardData, cardDrawnArgs.PlayerID);
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

    public void OnMythAttackedEvent(object args)
    {
        if (args.GetType() != typeof(MythAttackedEventArgs))
            return;

        MythAttackedEventArgs mythAttackedArgs = (MythAttackedEventArgs)args;

        GameCard attackingCard;

        if (NetworkManager.Singleton.LocalClientId.Equals(mythAttackedArgs.PlayerID))
        {
            attackingCard = myBoard.cards.Find(card => card.GameID == mythAttackedArgs.AttackingCardGameID);
        }
        else
        {
            attackingCard = opponentBoard.cards.Find(card => card.GameID == mythAttackedArgs.AttackingCardGameID);
        }

        if (attackingCard == null) return;

        // TODO: Do stuff with the card, animations and such

        UpdatePlayerInfoRpc();

        if (IsServer)
        {
            ClientCardManager.Instance.UpdateCardSnapshotRpc(mythAttackedArgs.AttackingCardGameID);
        }
    }

    public void OnCardDiedEvent(object args)
    {
        if (args.GetType() != typeof(GameCard))
            return;

        GameCard card = (GameCard)args;

        HorizontalCardHolder horizontalCardHolder = card.GetComponentInParent<HorizontalCardHolder>();
        horizontalCardHolder.RemoveCard(card);
        horizontalCardHolder.UpdateIndexes();

        RectTransform cardSlotTransform = card.transform.parent as RectTransform;

        if (ClientCardManager.Instance.CardBelongsToPlayer(NetworkManager.Singleton.LocalClientId, card.GameID)) 
        {
            cardSlotTransform.SetParent(myDiscardPile, false);
        } 
        else 
        {
            cardSlotTransform.transform.SetParent(oppDiscardPile, false);
        }

        cardSlotTransform.transform.localPosition = Vector3.zero;
        StartCoroutine(FlipCard(cardSlotTransform));
    }

    private IEnumerator FlipCard(Transform cardSlotTransform)
    {
        yield return new WaitForSeconds(0.5f);
        cardSlotTransform.gameObject.GetComponentInChildren<GameCard>().FlipCard();
    }

    public void Surrender()
    {
        GameplayManager.Instance.GameOverRpc(NetworkManager.Singleton.LocalClientId);
    }
}

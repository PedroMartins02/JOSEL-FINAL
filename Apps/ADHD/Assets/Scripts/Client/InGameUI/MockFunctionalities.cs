using Game.Logic.Actions;
using GameCore.Events;
using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockFunctionalities : MonoBehaviour
{
    /*
    [SerializeField] HorizontalCardHolder cardHolder;
    [SerializeField] PlayerInfo playerInfo;

    [SerializeField] DeckSO deckSO;
    [SerializeField] bool IsMine;

    private Deck deck;
    private Hand hand;

    private void Start()
    {
    }

    public void OpponentDrawCard()
    {
        cardHolder.SpawnCard(null);
    }

    public void DrawCardMock(string cardSOId)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(cardSOId);

        cardHolder.SpawnCard(cardSO);
    }

    public void DrawCardMock()
    {
        EventManager.Subscribe(GameEventsEnum.CardAddedToHand, SpawnCard);

        //DrawCardAction action = new(deck, hand, IsMine);

        //ActionQueueManager.Instance.AddAction(action);
    }

    public void SetPlayerMyth(string mythId)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(mythId);
        playerInfo.SetMythImage(cardSO.Art);
    }

    public void SetPlayerHealth(int n)
    {
        playerInfo.SetHealth(n);
    }

    public void SetPlayerBlessings(int n)
    {
        playerInfo.SetBlessings(n);
    }

    private void SpawnCard(object obj) 
    {
        EventManager.Unsubscribe(GameEventsEnum.CardAddedToHand, SpawnCard);

        /*
        if (obj is CardAddedToHandEventArgs args) 
        {
            cardHolder.SpawnCard(args.Card, args.IsMine);
        }
    }*/
}

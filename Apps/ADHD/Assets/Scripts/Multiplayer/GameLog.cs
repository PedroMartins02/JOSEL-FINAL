using System.Collections;
using System.Collections.Generic;
using Game.Multiplayer;
using GameCore.Events;
using GameModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Log
{
    public class GameLog : MonoBehaviour
    {
        public static GameLog Instance { get; private set; }

        private ulong playerID;

        public bool PlayerWon { get; private set; } = false;
        public int NumberOfCardsDrawn { get; private set; }
        public int NumberOfCardsAttacked { get; private set; }
        public int NumberOfAttackToOpponent { get; private set; }
        public int DamageReceived { get; private set; }
        public int DamageDealtToOpponent { get; private set; }
        public int DamageReceivedToCards { get; private set; }
        public int DamageDealtToCards { get; private set; }
        public int NumberOfCardsPlayed { get; private set; }
        public int CardsDestroyed { get; private set; }
        public int EnemyCardsDestroyed { get; private set; }

        public int OpponentMMR { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            EventManager.Subscribe(GameEventsEnum.CardDrawn, OnCardDrawnEvent);
            EventManager.Subscribe(GameEventsEnum.CardPlayed, OnCardPlayedEvent);
            EventManager.Subscribe(GameEventsEnum.CardAttacked, OnCardAttackedEvent);
            EventManager.Subscribe(GameEventsEnum.MythDamaged, OnMythAttackedEvent);
            EventManager.Subscribe(GameEventsEnum.CardDied, OnCardDiedEvent);
        }

        public void RegisterPlayerID(ulong clientId)
        {
            playerID = clientId;
        }

        public void SetPlayerWon(bool won)
        {
            PlayerWon = won;
        }
        
        public void SetOpponentMMR(int mmr)
        {
            OpponentMMR = mmr;
        }

        private void OnCardDrawnEvent(object args)
        {
            if (args.GetType() != typeof(CardDrawnEventArgs))
                return;

            CardDrawnEventArgs cardDrawnArgs = (CardDrawnEventArgs)args;

            if (cardDrawnArgs.PlayerID == playerID)
            {
                NumberOfCardsDrawn++;
            }
        }

        private void OnCardPlayedEvent(object args)
        {
            if (args.GetType() != typeof(CardPlayedEventArgs))
                return;

            CardPlayedEventArgs cardPlayedArgs = (CardPlayedEventArgs)args;

            if (cardPlayedArgs.PlayerID == playerID)
            {
                NumberOfCardsPlayed++;
            }
        }

        private void OnCardAttackedEvent(object args)
        {
            if (args.GetType() != typeof(CardAttackedEventArgs))
                return;

            CardAttackedEventArgs cardAttackedArgs = (CardAttackedEventArgs)args;

            if (cardAttackedArgs.PlayerID == playerID)
            {
                DamageDealtToCards += cardAttackedArgs.DamageDealt;
                NumberOfCardsAttacked++;
            }
            else
            {
                DamageReceivedToCards += cardAttackedArgs.DamageDealt;
            }
        }

        private void OnMythAttackedEvent(object args)
        {
            if (args.GetType() != typeof(MythAttackedEventArgs))
                return;

            MythAttackedEventArgs mythAttackedArgs = (MythAttackedEventArgs)args;


            if (mythAttackedArgs.PlayerID == playerID)
            {
                DamageDealtToOpponent += mythAttackedArgs.DamageDealt;
                NumberOfAttackToOpponent++;
            }
            else
            {
                DamageReceived += mythAttackedArgs.DamageDealt;
            }
        }

        private void OnCardDiedEvent(object args)
        {
            if (args.GetType() != typeof(GameCard))
                return;

            GameCard card = (GameCard)args;

            if (ClientCardManager.Instance.CardBelongsToPlayer(NetworkManager.Singleton.LocalClientId, card.GameID))
            {
                CardsDestroyed++;
            }
            else
            {
                EnemyCardsDestroyed++;
            }
        }
    }
}
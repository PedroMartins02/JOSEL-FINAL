using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class DrawCardAction : IAction
    {
        private Deck deck;
        private Hand hand;
        private int numberOfCards;

        private bool isMine;

        public DrawCardAction(Deck deck, Hand hand, bool isMine, int numberOfCards = 1)
        {
            this.deck = deck;
            this.hand = hand;
            this.numberOfCards = numberOfCards;

            this.isMine = isMine;
        }

        public bool IsLegal()
        {
            return true; // TODO: Adicionar condição para verificar se é o turno do jogador ou não
        }

        public IEnumerator Execute()
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                ICard drawnCard = deck.DrawCard();

                if (drawnCard != null)
                {
                    drawnCard.StateMachine.SetState(CardStateType.InHand);

                    bool addedToHand = hand.AddCard(drawnCard);

                    if (addedToHand)
                    {
                        CardAddedToHandEventArgs args = new(drawnCard, isMine);
                        EventManager.TriggerEvent(GameEventsEnum.CardAddedToHand, args);
                    }
                    else
                    {
                        Debug.Log("Hand is full. Cannot add card");
                    }
                }
            }

            yield return null;
        }
    }

    public readonly struct CardAddedToHandEventArgs
    {
        public readonly ICard Card;
        public readonly bool IsMine;

        public CardAddedToHandEventArgs(ICard card, bool isMine)
        {
            this.Card = card;
            this.IsMine = isMine;
        }
    }
}


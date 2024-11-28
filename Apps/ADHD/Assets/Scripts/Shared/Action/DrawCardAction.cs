using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class DrawCardAction : IAction
    {
        private Player player;
        private int numberOfCards;

        public DrawCardAction(Player player, int numberOfCards = 1)
        {
            this.player = player;
            this.numberOfCards = numberOfCards;
        }

        public bool IsLegal()
        {
            return true; // TODO: Adicionar condição para verificar se é o turno do jogador ou não
        }

        public void Execute()
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                ICard drawnCard = player.Deck.DrawCard();

                if (drawnCard != null)
                {
                    drawnCard.StateMachine.SetState(CardStateType.InHand);

                    bool addedToHand = player.Hand.AddCard(drawnCard);

                    if (addedToHand)
                    {
                        ActionData actionData = new ActionData
                        {
                            ActionType = ActionType.DrawCard,
                            PlayerId = player.playerData.ClientId,
                            CardId = drawnCard.Data.Id,
                        };

                        GameplayManager.Instance.BroadcastActionExecuted(actionData); // Notify Clients

                        EventManager.TriggerEvent(GameEventsEnum.CardAddedToHand, actionData); // Notify the rest of the server/backend
                    }
                    else
                    {
                        Debug.Log("Hand is full. Cannot add card");
                    }
                }
            }
        }
    }
}


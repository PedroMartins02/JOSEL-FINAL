using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public class DiscardedState : IState<CardStateType, CardActions>
    {
        private Card card;
        public CardStateType StateType => CardStateType.Discarded;

        public DiscardedState(Card card)
        {
            this.card = card;
        }

        public void EnterState()
        {

        }

        public void UpdateState()
        {

        }

        public void ExitState()
        {

        }

        public void OnAction(CardActions action)
        {

        }
    }
}

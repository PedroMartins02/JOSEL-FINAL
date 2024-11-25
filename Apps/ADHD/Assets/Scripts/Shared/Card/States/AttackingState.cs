using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public class AttackingState : IState<CardStateType, CardActions>
    {
        private Card card;
        public CardStateType StateType => CardStateType.Attacking;

        public AttackingState(Card card)
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
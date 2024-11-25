using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public class InPlayState : IState<CardStateType, CardActions>
    {
        public virtual CardStateType StateType => CardStateType.InPlay;

        protected readonly Card card;

        public InPlayState(Card card)
        {
            this.card = card;
        }

        public virtual void EnterState()
        {

        }

        public virtual void UpdateState()
        {

        }

        public virtual void ExitState()
        {

        }

        public virtual void OnAction(CardActions action)
        {
            switch (action)
            {
                case CardActions.Attack:
                    card.StateMachine.SetState(CardStateType.Exhausted);
                    break;
            }
        }
    }
}
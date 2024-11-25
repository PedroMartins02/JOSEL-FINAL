using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
using GameModel;

namespace Game.Logic
{
    public class InDeckState : IState<CardStateType, CardActions>
    {
        private Card card;
        public CardStateType StateType => CardStateType.InDeck;

        public InDeckState(Card card)
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

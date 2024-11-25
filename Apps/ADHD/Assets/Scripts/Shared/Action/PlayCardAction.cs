using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class PlayCardAction : IAction
    {
        private ICard card;

        public PlayCardAction(ICard card)
        {
            this.card = card;
        }

        public bool IsLegal()
        {
            return true;
        }

        public IEnumerator Execute()
        {
            card.StateMachine.SetState(CardStateType.InPlay);

            yield return null;
        }
    }
}

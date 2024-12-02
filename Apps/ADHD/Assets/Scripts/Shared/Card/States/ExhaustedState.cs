using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
using GameCore.Events;
using GameModel;
using UnityEngine;

namespace Game.Logic
{
    public class ExhaustedState : InPlayState
    {
        public override CardStateType StateType => CardStateType.Exhausted;

        private int turnsExhausted = 0;

        public ExhaustedState(Card card) : base(card) { }

        public override void EnterState()
        {
            EventManager.Subscribe(GameEventsEnum.TurnStarted, HandleTurnStartedEvent);
            turnsExhausted = 0;
        }

        public override void UpdateState()
        {

        }

        public override void ExitState()
        {
            EventManager.Unsubscribe(GameEventsEnum.TurnStarted, HandleTurnStartedEvent);
        }

        public override void OnAction(CardActions action)
        {
            
        }

        private void HandleTurnStartedEvent(object args) 
        {
            turnsExhausted++;

            if (turnsExhausted == 2)
                card.StateMachine.SetState(CardStateType.InPlay);
        }
    }
}
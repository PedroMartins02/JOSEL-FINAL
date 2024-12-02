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

        public ExhaustedState(Card card) : base(card) { }

        public override void EnterState()
        {
            EventManager.Subscribe(GameEventsEnum.TurnStarted, HandleTurnStartedEvent);
        }

        public override void UpdateState()
        {

        }

        public override void ExitState()
        {
            EventManager.Unsubscribe(GameEventsEnum.TurnStarted, HandleTurnStartedEvent);

            card.StateMachine.SetState(CardStateType.InPlay);
        }

        public override void OnAction(CardActions action)
        {
            
        }

        private void HandleTurnStartedEvent(object args) 
        {
            if (args == null || args.GetType() == typeof(ulong)) return;

            ulong playerID = (ulong)args;

            Player player = PlayerManager.Instance.GetPlayerByClientId(playerID);

            if (player == null || !player.HasCardOnField(card)) return;

            ExitState();
        }
    }
}
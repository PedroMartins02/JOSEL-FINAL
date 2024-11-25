using System.Collections;
using System.Collections.Generic;
using Game.Utils.Logic;
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

        }

        public override void UpdateState()
        {

        }

        public override void ExitState()
        {

        }

        public override void OnAction(CardActions action)
        {
            
        }
    }
}
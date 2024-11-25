using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils.Logic
{
    public interface IState<TStateType, TAction> where TStateType : Enum
    {
        TStateType StateType { get; }
        void EnterState();
        void ExitState();
        void UpdateState();
        void OnAction(TAction action);

    }
}
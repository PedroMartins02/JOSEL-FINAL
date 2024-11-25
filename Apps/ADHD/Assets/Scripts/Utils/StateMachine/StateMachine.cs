using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils.Logic
{
    public class StateMachine<TStateType, TActionType> where TStateType : Enum
    {
        public event Action<TStateType> OnStateChanged;

        public IState<TStateType, TActionType> CurrentState { get; private set; }

        private Dictionary<TStateType, IState<TStateType, TActionType>> states = new Dictionary<TStateType, IState<TStateType, TActionType>>();

        public void AddState(IState<TStateType, TActionType> state)
        {
            states[state.StateType] = state;
        }

        public void RemoveState(IState<TStateType, TActionType> state)
        {
            states.Remove(state.StateType);
        }

        public void SetState(TStateType newStateType)
        {
            CurrentState?.ExitState();
            CurrentState = states[newStateType];
            CurrentState.EnterState();

            OnStateChanged?.Invoke(newStateType);
        }


        public void Update()
        {
            CurrentState?.UpdateState();
        }

        public void HandleAction(TActionType action)
        {
            CurrentState?.OnAction(action);
        }
    }
}

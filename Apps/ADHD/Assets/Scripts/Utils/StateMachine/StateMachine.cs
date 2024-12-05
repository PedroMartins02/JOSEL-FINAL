using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils.Logic
{
    public class StateMachine<TStateType, TActionType> where TStateType : Enum
    {
        public event Action<TStateType, TStateType> OnStateChanged;

        public IState<TStateType, TActionType> CurrentState { get; private set; }

        private Dictionary<TStateType, IState<TStateType, TActionType>> states = new Dictionary<TStateType, IState<TStateType, TActionType>>();

        public virtual void AddState(IState<TStateType, TActionType> state)
        {
            states[state.StateType] = state;
        }

        public virtual void RemoveState(IState<TStateType, TActionType> state)
        {
            states.Remove(state.StateType);
        }

        public virtual void SetState(TStateType newStateType)
        {
            if (!states.ContainsKey(newStateType)) return;

            TStateType oldState = CurrentState != null ? CurrentState.StateType : default(TStateType);

            CurrentState?.ExitState();
            CurrentState = states[newStateType];
            CurrentState.EnterState();

            OnStateChanged?.Invoke(oldState, newStateType);
        }


        public virtual void Update()
        {
            CurrentState?.UpdateState();
        }

        public virtual void HandleAction(TActionType action)
        {
            CurrentState?.OnAction(action);
        }
    }
}

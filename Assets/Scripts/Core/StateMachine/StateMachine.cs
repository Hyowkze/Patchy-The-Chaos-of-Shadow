using UnityEngine;
using System.Collections.Generic;

namespace Core.StateMachine
{
    public class StateMachine<TState, TContext> where TState : System.Enum
    {
        private readonly Dictionary<TState, IState<TContext>> states = new Dictionary<TState, IState<TContext>>();
        private readonly TContext context;
        private IState<TContext> currentState;
        
        public TState CurrentStateType { get; private set; }

        public StateMachine(TContext context)
        {
            this.context = context;
        }

        public void AddState(TState stateKey, IState<TContext> state)
        {
            states[stateKey] = state;
        }

        public void ChangeState(TState newStateType)
        {
            if (!states.ContainsKey(newStateType)) 
            {
                Debug.LogError($"State {newStateType} not found!");
                return;
            }

            currentState?.Exit();
            CurrentStateType = newStateType;
            currentState = states[newStateType];
            currentState.Enter();
        }

        public void UpdateState()
        {
            currentState?.Update();
        }

        public void FixedUpdateState()
        {
            currentState?.FixedUpdate();
        }
    }

    public interface IState<TContext>
    {
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }
}

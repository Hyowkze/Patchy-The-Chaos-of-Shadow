using System;
using System.Collections.Generic;
using Core.Player.Movement;
using Core.Interfaces;
using Core.Player.Movement.States;
using UnityEngine;

namespace Player.Movement
{
    public class MovementStateFactory
    {
        private readonly Dictionary<MovementStateMachine.MovementState, IMovementState> states = new Dictionary<MovementStateMachine.MovementState, IMovementState>();
        private readonly MovementStateMachine stateMachine; // Add a reference to the state machine

        public MovementStateFactory(PatchyMovement movement, MovementConfig config)
        {
            Rigidbody2D rb = movement.GetComponent<Rigidbody2D>(); // Get Rigidbody2D here
            stateMachine = movement.GetComponent<MovementStateMachine>(); // Get the MovementStateMachine

            states.Add(MovementStateMachine.MovementState.Idle, new IdleState(movement, rb, config)); // Pass rb and config
            states.Add(MovementStateMachine.MovementState.Walking, new WalkingState(movement, config));
            states.Add(MovementStateMachine.MovementState.Jumping, new JumpingState(movement, rb, config)); // Pass rb and config
            states.Add(MovementStateMachine.MovementState.Dashing, new DashingState(movement, rb, config, stateMachine)); // Pass rb, config, and stateMachine
            states.Add(MovementStateMachine.MovementState.Sprinting, new SprintingState(movement, rb, config)); // Pass rb and config
        }

        public IMovementState GetState(MovementStateMachine.MovementState state)
        {
            if (states.ContainsKey(state))
            {
                return states[state];
            }
            else
            {
                throw new ArgumentException($"Invalid state: {state}");
            }
        }
    }
}

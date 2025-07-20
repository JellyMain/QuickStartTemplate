using Infrastructure.GameStates.Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Infrastructure.Services
{
    public class GameStateMachine
    {
        private readonly ContainerService containerService;
        private IState currentState;


        public GameStateMachine(ContainerService containerService)
        {
            this.containerService = containerService;
        }


        public void Enter<TState>() where TState : class, IEnterableState, IState
        {
            TState newState = CreateOrGetState<TState>();

            if (currentState is IExitableState exitableState)
            {
                exitableState.Exit();
            }

            currentState = newState;

            newState.Enter();
        }


        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>, IState
        {
            TState newState = CreateOrGetState<TState>();

            if (currentState is IExitableState exitableState)
            {
                exitableState.Exit();
            }

            currentState = newState;

            newState.Enter(payload);
        }
        


        private TState CreateOrGetState<TState>() where TState : class, IState
        {
            TState state;

            if (containerService.GlobalContainer.Container.TryResolve(out TState resolved))
            {
                state = resolved;
            }
            else
            {
                state = containerService.LocalContainer.Container.Resolve<TState>();
            }


            if (state == null)
            {
                Debug.LogError("Game state is not bind or resolved correctly");
            }

            return state;
        }
    }
}

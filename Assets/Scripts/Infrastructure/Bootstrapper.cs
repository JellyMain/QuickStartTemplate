using Infrastructure.GameStates;
using Infrastructure.Services;
using UnityEngine;
using VContainer.Unity;


namespace Infrastructure
{
    public class Bootstrapper : IStartable
    {
        private readonly GameStateMachine gameStateMachine;


        public Bootstrapper(GameStateMachine gameStateMachine)
        {
            this.gameStateMachine = gameStateMachine;
        }


        public void Start()
        {
            gameStateMachine.Enter<BootstrapState>();
        }
    }
}

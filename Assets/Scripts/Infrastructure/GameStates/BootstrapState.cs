using Infrastructure.GameStates.Interfaces;
using Infrastructure.Services;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class BootstrapState : IEnterableState
    {
        private readonly GameStateMachine gameStateMachine;


        public BootstrapState(GameStateMachine gameStateMachine)
        {
            this.gameStateMachine = gameStateMachine;
        }


        public void Enter()
        {
            Debug.Log("Entered BootstrapState");
            gameStateMachine.Enter<LoadProgressState>();
        }
    }
}

using Infrastructure.GameStates.Interfaces;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class GameLoopState : IEnterableState
    {
        public GameLoopState() { }


        public void Enter()
        {
            Debug.Log("Entered GameLoopState");
        }
    }
}

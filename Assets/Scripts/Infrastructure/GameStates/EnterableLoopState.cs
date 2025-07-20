using Infrastructure.GameStates.Interfaces;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class EnterableLoopState : IEnterableState
    {
        public EnterableLoopState() { }


        public void Enter()
        {
            Debug.Log("Entered GameLoopState");
            InitGameLoopServices();
        }


        private void InitGameLoopServices() { }
    }
}




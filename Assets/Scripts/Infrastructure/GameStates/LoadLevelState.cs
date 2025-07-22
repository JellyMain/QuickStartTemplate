using Factories;
using Infrastructure.GameStates.Interfaces;
using Infrastructure.Services;
using Progress;
using Scenes;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class LoadLevelState : IEnterableState
    {
        private readonly SceneLoader sceneLoader;
        private readonly GameplayUIFactory gameplayUIFactory;
        private readonly SaveLoadService saveLoadService;
        private readonly GameStateMachine gameStateMachine;


        public LoadLevelState(SceneLoader sceneLoader, GameplayUIFactory gameplayUIFactory,
            SaveLoadService saveLoadService, GameStateMachine gameStateMachine)
        {
            this.sceneLoader = sceneLoader;
            this.gameplayUIFactory = gameplayUIFactory;
            this.saveLoadService = saveLoadService;
            this.gameStateMachine = gameStateMachine;
        }


        public void Enter()
        {
            Debug.Log("Entered LoadLevelState");
            CreateLevel();
            saveLoadService.UpdateProgress();

            gameStateMachine.Enter<GameLoopState>();
        }


        private void CreateLevel() { }
    }
}

using Const;
using Infrastructure.GameStates.Interfaces;
using Infrastructure.Services;
using Progress;
using Scenes;
using StaticData.Services;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class LoadProgressState : IEnterableState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly StaticDataService staticDataService;
        private readonly SaveLoadService saveLoadService;
        private readonly PersistentPlayerProgress persistentPlayerProgress;
        private readonly SceneLoader sceneLoader;


        public LoadProgressState(GameStateMachine gameStateMachine, StaticDataService staticDataService,
            SaveLoadService saveLoadService, PersistentPlayerProgress persistentPlayerProgress, SceneLoader sceneLoader)
        {
            this.gameStateMachine = gameStateMachine;
            this.staticDataService = staticDataService;
            this.saveLoadService = saveLoadService;
            this.persistentPlayerProgress = persistentPlayerProgress;
            this.sceneLoader = sceneLoader;
        }


        public async void Enter()
        {
            Debug.Log("Entered LoadProgressState");
            await staticDataService.LoadStaticData();
            LoadSavesOrCreateNew();
            sceneLoader.Load(RuntimeConstants.Scenes.MAIN_MENU_SCENE, () => gameStateMachine.Enter<LoadMetaState>());
        }


        private void LoadSavesOrCreateNew()
        {
            persistentPlayerProgress.PlayerProgress = LoadOrCreateNewProgress();
        }



        private PlayerProgress LoadOrCreateNewProgress()
        {
            PlayerProgress playerProgress = saveLoadService.LoadProgress() ?? new PlayerProgress();
            return playerProgress;
        }
    }
}

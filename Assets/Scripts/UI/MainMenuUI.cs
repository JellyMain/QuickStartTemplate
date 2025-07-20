using Const;
using Infrastructure.GameStates;
using Infrastructure.Services;
using Scenes;
using UnityEngine;
using VContainer;


namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private SceneLoader sceneLoader;
        private GameStateMachine gameStateMachine;


        [Inject]
        private void Construct(SceneLoader sceneLoader, GameStateMachine gameStateMachine)
        {
            this.sceneLoader = sceneLoader;
            this.gameStateMachine = gameStateMachine;
        }


        public void StartGame()
        {
            sceneLoader.Load(RuntimeConstants.Scenes.GAME_SCENE, () => gameStateMachine.Enter<LoadLevelState>());
        }
    }
}

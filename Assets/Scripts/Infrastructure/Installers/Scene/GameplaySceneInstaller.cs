using System.Collections;
using System.Collections.Generic;
using Factories;
using Infrastructure.GameStates;
using UnityEngine;
using VContainer;



namespace Infrastructure.Installers.Scene
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            BindGameplayUIFactory(builder);
            BindGameStates(builder);
        }



        private void BindGameplayUIFactory(IContainerBuilder builder)
        {
            builder.Register<GameplayUIFactory>(Lifetime.Singleton);
        }


        private void BindGameStates(IContainerBuilder builder)
        {
            builder.Register<LoadLevelState>(Lifetime.Singleton);
            builder.Register<GameLoopState>(Lifetime.Singleton);
        }
    }
}



using Assets;
using Infrastructure.Services;
using Progress;
using Scenes;
using StaticData.Services;
using UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Infrastructure.Installers.Global
{
    public class InfrastructureInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreen loadingScreenPrefab;


        public override void Install(IContainerBuilder builder)
        {
            BindBootstrapper(builder);
            BindPersistentPlayerProgress(builder);
            BindSaveLoadService(builder);
            BindStaticDataService(builder);
            BindContainerService(builder);
            BindAssetProvider(builder);
            BindSceneLoader(builder);
            CreateAndBindLoadingScreen(builder);
        }

        

        private void BindBootstrapper(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Bootstrapper>();
        }


        private void BindPersistentPlayerProgress(IContainerBuilder builder)
        {
            builder.Register<PersistentPlayerProgress>(Lifetime.Singleton);
        }


        private void BindSaveLoadService(IContainerBuilder builder)
        {
            builder.Register<SaveLoadService>(Lifetime.Singleton);
        }


        private void BindStaticDataService(IContainerBuilder builder)
        {
            builder.Register<StaticDataService>(Lifetime.Singleton);
        }


        private void BindContainerService(IContainerBuilder builder)
        {
            builder.Register<ContainerService>(Lifetime.Singleton);
        }


        private void BindAssetProvider(IContainerBuilder builder)
        {
            builder.Register<AssetProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }


        private void BindSceneLoader(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Singleton);
        }


        private void CreateAndBindLoadingScreen(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(loadingScreenPrefab, Lifetime.Singleton);
        }
    }
}

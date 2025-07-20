using Factories;
using Infrastructure.GameStates;
using VContainer;


namespace Infrastructure.Installers.Scene
{
    public class MenuSceneInstaller : MonoInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            BindLoadMetaState(builder);
            BindMetaUIFactory(builder);
        }


        private void BindLoadMetaState(IContainerBuilder builder)
        {
            builder.Register<LoadMetaState>(Lifetime.Singleton);
        }


        private void BindMetaUIFactory(IContainerBuilder builder)
        {
            builder.Register<MetaUIFactory>(Lifetime.Singleton);
        }
    }
}

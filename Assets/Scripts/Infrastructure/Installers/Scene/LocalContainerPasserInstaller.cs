using Infrastructure.GameStates.Interfaces;
using Infrastructure.Services;
using VContainer;
using VContainer.Unity;


namespace Infrastructure.Installers.Scene
{
    public class LocalContainerPasserInstaller : MonoInstaller
    {
        public override void Install(IContainerBuilder builder)
        {
            BindLocalContainerPasser(builder);
        }

        
        private void BindLocalContainerPasser(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LocalContainerPasser>();
        }
    }
}

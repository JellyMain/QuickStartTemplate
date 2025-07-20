

using VContainer.Unity;


namespace Infrastructure.Services
{
    public class ContainerService 
    {
        public LifetimeScope LocalContainer { get; private set; }
        public LifetimeScope GlobalContainer { get; private set; }
        
        
        public ContainerService(LifetimeScope globalDiContainer)
        {
            GlobalContainer = globalDiContainer;
        }
        
        
        public void SetLocalContainer(LifetimeScope container)
        {
            LocalContainer = container;
        }
    }
}

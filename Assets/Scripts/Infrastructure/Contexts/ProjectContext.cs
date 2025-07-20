using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Infrastructure.Contexts
{
    public class ProjectContext : LifetimeScope
    {
        [SerializeField] private List<MonoInstaller> projectInstallers;

        
        protected override void Configure(IContainerBuilder builder)
        {
            foreach (MonoInstaller installer in projectInstallers)
            {
                installer.Install(builder);
            }
        }
    }
}
using Infrastructure.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;


namespace Cheats
{
    public class CheatsService : MonoBehaviour
    {
        private ContainerService containerService;


        [Inject]
        private void Construct(ContainerService containerService)
        {
            this.containerService = containerService;
        }


        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}

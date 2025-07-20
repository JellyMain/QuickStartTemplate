using Factories;
using Infrastructure.GameStates.Interfaces;
using Progress;
using UnityEngine;


namespace Infrastructure.GameStates
{
    public class LoadMetaState : IEnterableState
    {
        private readonly SaveLoadService saveLoadService;
        private readonly MetaUIFactory metaUIFactory;


        public LoadMetaState(SaveLoadService saveLoadService, MetaUIFactory metaUIFactory)
        {
            this.saveLoadService = saveLoadService;
            this.metaUIFactory = metaUIFactory;
        }


        public void Enter()
        {
            Debug.Log("Entered LoadMetaState");
            saveLoadService.UpdateProgress();
        }
    }
}
using System;
using UnityEngine;
using VContainer;


namespace Progress
{
    public class SessionSaveService : MonoBehaviour
    {
        private SaveLoadService saveLoadService;


        [Inject]
        private void Construct(SaveLoadService saveLoadService)
        {
            this.saveLoadService = saveLoadService;
        }

        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        private void OnApplicationQuit()
        {
            saveLoadService.SaveProgress();
        }


        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                saveLoadService.SaveProgress();
            }
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                saveLoadService.SaveProgress();
            }
        }
    }
}

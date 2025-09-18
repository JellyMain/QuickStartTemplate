using Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Factories
{
    public abstract class BaseFactory : IInitializable
    {
        private readonly IObjectResolver objectResolver;
        private readonly AssetProvider assetProvider;


        protected BaseFactory(IObjectResolver objectResolver, AssetProvider assetProvider)
        {
            this.objectResolver = objectResolver;
            this.assetProvider = assetProvider;
        }


        public void Initialize()
        {
            WarmUpPrefabs();
        }


        protected abstract void WarmUpPrefabs();


        protected void WarmUpPrefab(string prefabAddress)
        {
            PreloadPrefab(prefabAddress).Forget();
        }


        protected async UniTaskVoid PreloadPrefab(string prefabAddress)
        {
            await assetProvider.LoadAsset<GameObject>(prefabAddress);
        }


        protected async UniTask<GameObject> InstantiatePrefab(string prefabAddress, Transform parent = null)
        {
            GameObject prefab = await assetProvider.LoadAsset<GameObject>(prefabAddress);
            return objectResolver.Instantiate(prefab, parent);
        }


        protected async UniTask<GameObject> InstantiatePrefab(string prefabAddress, Vector3 position,
            Transform parent = null)
        {
            GameObject prefab = await assetProvider.LoadAsset<GameObject>(prefabAddress);
            return objectResolver.Instantiate(prefab, position, Quaternion.identity, parent);
        }


        protected async UniTask<GameObject> InstantiatePrefab(string prefabAddress, Vector3 position,
            Quaternion rotation, Transform parent = null)
        {
            GameObject prefab = await assetProvider.LoadAsset<GameObject>(prefabAddress);
            return objectResolver.Instantiate(prefab, position, rotation, parent);
        }


        protected async UniTask<T> InstantiatePrefabWithComponent<T>(string prefabAddress, Transform parent = null)
            where T : Component
        {
            GameObject instance = await InstantiatePrefab(prefabAddress, parent);
            return instance.GetComponent<T>();
        }


        protected async UniTask<T> InstantiatePrefabWithComponent<T>(string prefabAddress, Vector3 position,
            Transform parent = null) where T : Component
        {
            GameObject instance = await InstantiatePrefab(prefabAddress, position, parent);
            return instance.GetComponent<T>();
        }


        protected async UniTask<T> InstantiatePrefabWithComponent<T>(string prefabAddress, Vector3 position,
            Quaternion rotation, Transform parent = null) where T : Component
        {
            GameObject instance = await InstantiatePrefab(prefabAddress, position, rotation, parent);
            return instance.GetComponent<T>();
        }
    }
}

using Assets;
using VContainer;


namespace Factories
{
    public class GameplayUIFactory : BaseFactory

    {
        public GameplayUIFactory(IObjectResolver objectResolver, AssetProvider assetProvider) : base(objectResolver,
            assetProvider) { }


        protected override void WarmUpPrefabs() { }
    }
}

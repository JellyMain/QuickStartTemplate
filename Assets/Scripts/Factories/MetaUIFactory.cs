using Assets;
using Const;
using Cysharp.Threading.Tasks;
using Progress;
using UnityEngine;
using VContainer;


namespace Factories
{
    public class MetaUIFactory : BaseFactory
    {
        public MetaUIFactory(AssetProvider assetProvider, IObjectResolver objectResolver) :
            base(objectResolver, assetProvider) { }


        protected override void WarmUpPrefabs() { }
    }
}

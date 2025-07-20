using Assets;
using Cysharp.Threading.Tasks;


namespace StaticData.Services
{
    public class StaticDataService
    {
        private readonly AssetProvider assetProvider;
        
        

        public StaticDataService(AssetProvider assetProvider)
        {
            this.assetProvider = assetProvider;
        }


        public async UniTask LoadStaticData()
        {
            
        }


       
    }
}
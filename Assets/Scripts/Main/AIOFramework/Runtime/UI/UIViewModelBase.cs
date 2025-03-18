using System.Collections.Generic;
using AIOFramework.Resource;
using Loxodon.Framework.ViewModels;
using AIOFramework.Runtime;
using Cysharp.Threading.Tasks;

namespace AIOFramework.UI
{
    public abstract class UIViewModelBase : ViewModelBase, IReference
    {
        private IAssetManager m_AssetManager;
        private List<string> m_LoadedAssets = new List<string>();

        public void SetAssetManager(IAssetManager assetManager)
        {
            if (m_AssetManager != null) return;
            m_AssetManager = assetManager;
        }

        protected async UniTask<T> LoadAsset<T>(string location) where T : UnityEngine.Object
        {
            try
            {
                if (m_AssetManager == null)
                {
                    Log.Error("AssetManager is null");
                    return null;
                }

                T asset = await m_AssetManager.LoadAssetAsync<T>(location);
                if (asset != null)
                {
                    m_LoadedAssets.Add(location);
                }

                return asset;
            }
            catch (System.Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public virtual void Clear()
        {
            if (m_AssetManager != null)
            {
                foreach (var asset in m_LoadedAssets)
                {
                    m_AssetManager.UnloadAsset(asset);
                }
            }

            Dispose();
        }
    }
}
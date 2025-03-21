using System.Collections.Generic;
using AIOFramework.Resource;
using Loxodon.Framework.ViewModels;
using AIOFramework.Runtime;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace AIOFramework.UI
{
    public abstract class UIViewModelBase : ViewModelBase, IReference
    {
        private IAssetManager m_AssetManager;
        private List<HandleBase> m_LoadedAssets = new List<HandleBase>();

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

                var result = await m_AssetManager.LoadAssetAsync<T>(location);
                if (result.Item2 != null)
                {
                    m_LoadedAssets.Add(result.Item2);
                }

                return result.Item1;
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
                foreach (HandleBase handle in m_LoadedAssets)
                {
                    m_AssetManager.UnloadAsset(handle);
                }
            }

            Dispose();
        }
    }
}
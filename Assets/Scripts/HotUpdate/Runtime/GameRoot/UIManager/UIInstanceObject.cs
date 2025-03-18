using AIOFramework.ObjectPool;
using UnityEngine;
using AIOFramework.Resource;

namespace AIOFramework.Runtime
{
    public class UIInstanceObject : ObjectBase
    {
        private GameObject m_UIInstance;
        private string location;
        private IAssetManager m_AssetManager;
        
        public UIInstanceObject()
        {
            m_UIInstance = null;
            m_AssetManager = null;
        }
        
        public static UIInstanceObject Create(string name, string location, GameObject m_UIInstance, IAssetManager assetMgr)
        {
            if (location == null)
            {
                throw new GameFrameworkException("UI form asset is invalid.");
            }

            if (assetMgr == null)
            {
                throw new GameFrameworkException("UI form helper is invalid.");
            }

            UIInstanceObject instanceObj = ReferencePool.Acquire<UIInstanceObject>();
            instanceObj.Initialize(name, m_UIInstance);
            instanceObj.m_UIInstance = m_UIInstance;
            instanceObj.m_AssetManager = assetMgr;
            instanceObj.location = location;
            return instanceObj;
        }
        
        
        public override void Clear()
        {
            base.Clear();
            m_UIInstance = null;
            m_AssetManager = null;
        }
        
        protected internal override void Release(bool isShutdown)
        {
            m_AssetManager.DestroyInstance(m_UIInstance);
            m_AssetManager.UnloadAsset(location);
        }
    }
}
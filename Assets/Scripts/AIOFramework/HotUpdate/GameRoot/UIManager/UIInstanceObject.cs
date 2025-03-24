using AIOFramework.ObjectPool;
using UnityEngine;
using AIOFramework.Resource;
using HotUpdate;
using YooAsset;

namespace AIOFramework.Runtime
{
    public class UIInstanceObject : ObjectBase
    {
        private HandleBase _handle;

        public static UIInstanceObject Create(string name, string location, GameObject m_UIInstance, HandleBase handle)
        {
            if (location == null)
            {
                throw new GameFrameworkException("UI form asset is invalid.");
            }
            
            UIInstanceObject instanceObj = ReferencePool.Acquire<UIInstanceObject>();
            instanceObj.Initialize(name, m_UIInstance);
            instanceObj._handle = handle;
            return instanceObj;
        }
        
        
        protected internal override void Release(bool isShutdown)
        {
            Entrance.Resource.DestroyInstance(Target as GameObject);
            Entrance.Resource.UnloadAsset(_handle);
        }
    }
}
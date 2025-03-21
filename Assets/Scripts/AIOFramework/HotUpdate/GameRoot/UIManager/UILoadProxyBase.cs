using System;
using System.Collections.Generic;
using AIOFramework.Runtime;
using HotUpdate;
using UnityEngine;
using YooAsset;

namespace AIOFramework.UI
{
    public abstract class UILoadProxyBase : MonoBehaviour, IUILoadProxy
    {
        public List<HandleBase> Handles { get; set; } = new List<HandleBase>();

        public abstract void Load(string location);

        public virtual void OnDispose()
        {
            foreach (var handle in Handles)
            {
                Entrance.Resource.UnloadAsset(handle);
            }
        }

        private void OnDestroy()
        {
            OnDispose();
        }
    }
}
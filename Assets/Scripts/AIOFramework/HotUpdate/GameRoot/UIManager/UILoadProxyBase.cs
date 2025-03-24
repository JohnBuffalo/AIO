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
        private string _curLocation;
        
        public virtual string CurLocation
        {
            get { return _curLocation; }
            set
            {
                if (_curLocation == value) return;
                _curLocation = value;
                OnLocationChange();
            }
        }
        public abstract void OnLocationChange();

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
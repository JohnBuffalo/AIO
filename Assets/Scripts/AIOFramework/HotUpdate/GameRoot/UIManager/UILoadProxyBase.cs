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
        protected string curLocation;
        public List<HandleBase> Handles { get; set; } = new List<HandleBase>();
        public virtual string CurLocation
        {
            get { return curLocation; }
            set
            {
                if (curLocation == value) return;
                curLocation = value;
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
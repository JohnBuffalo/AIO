using HotUpdate;
using UnityEngine;

namespace AIOFramework.UI
{
    public class UIPrefabLoadProxy : UILoadProxyBase
    {
        private string prefabPath;

        public override async void Load(string location)
        {
            var result = await Game.Resource.InstantiateAsync<GameObject>(location, transform);
            Handles.Add(result.Item2);
        }

        public string PrefabPath
        {
            get { return prefabPath; }
            set
            {
                if (prefabPath == value) return;
                prefabPath = value;
                if (string.IsNullOrEmpty(prefabPath)) return;
                Load(prefabPath);
            }
        }
    }
}
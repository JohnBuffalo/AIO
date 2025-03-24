using HotUpdate;
using UnityEngine;

namespace AIOFramework.UI
{
    public class UIPrefabLoadProxy : UILoadProxyBase
    {

        public override async void OnLocationChange()
        {
            var result = await Game.Resource.InstantiateAsync<GameObject>(curLocation, transform);
            Handles.Add(result.Item2);
        }

        public override string CurLocation
        {
            get { return curLocation; }
            set
            {
                if (curLocation == value) return;
                curLocation = value;
                if (string.IsNullOrEmpty(curLocation)) return;
                OnLocationChange();
            }
        }
    }
}
using HotUpdate;
using UnityEngine;

namespace AIOFramework.UI
{
    public class UIPrefabLoadProxy : UILoadProxyBase
    {
        private string _curLocation;
        
        public override async void OnLocationChange()
        {
            var result = await Game.Resource.InstantiateAsync<GameObject>(_curLocation, transform);
            Handles.Add(result.Item2);
        }

        public override string CurLocation
        {
            get { return _curLocation; }
            set
            {
                if (_curLocation == value) return;
                _curLocation = value;
                if (string.IsNullOrEmpty(_curLocation)) return;
                OnLocationChange();
            }
        }
    }
}
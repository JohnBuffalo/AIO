using System;
using AIOFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace AIOFramework.UI
{
    /// <summary>
    /// UI模块Sprite加载器
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UISpriteLoadProxy : UILoadProxyBase
    {
        private Image target;

        [SerializeField]
        private Sprite defaultSprite;
        [SerializeField]
        private Material defaultMaterial;


        public override string CurLocation
        {
            get { return _curLocation; }
            set
            {
                if (_curLocation == value)
                    return;
                _curLocation = value;
                if (string.IsNullOrEmpty(_curLocation))
                {
                    target.sprite = defaultSprite;
                    target.material = defaultMaterial;
                    return;
                }
                OnLocationChange();
            }
        }

        public override async void OnLocationChange()
        {
            try
            {
                var result = await Entrance.Resource.LoadAssetAsync<Sprite>(_curLocation);
                Handles.Add(result.Item2);
                target.sprite = result.Item1;
            }
            catch (Exception e)
            {
                Log.Error("Loading sprite failed", e.Message);
            }
        }

        private void Awake()
        {
            target = GetComponent<Image>();
        }

    }
}
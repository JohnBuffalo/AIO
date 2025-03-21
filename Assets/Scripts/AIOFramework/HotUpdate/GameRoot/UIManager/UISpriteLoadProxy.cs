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
        private string spritePath;
        [SerializeField]
        private Sprite defaultSprite;
        [SerializeField]
        private Material defaultMaterial;
        
        public string SpritePath
        {
            get { return spritePath; }
            set
            {
                if (spritePath == value)
                    return;
                spritePath = value;
                if (string.IsNullOrEmpty(spritePath))
                {
                    target.sprite = defaultSprite;
                    target.material = defaultMaterial;
                    return;
                }
                Load(spritePath);
            }
        }

        public override async void Load(string location)
        {
            try
            {
                var result = await Entrance.Resource.LoadAssetAsync<Sprite>(location);
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
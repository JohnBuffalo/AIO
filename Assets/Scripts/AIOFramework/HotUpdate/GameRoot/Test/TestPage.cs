using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using TMPro;
using AIOFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AIOFramework.Runtime
{
    public class TestPage : UIViewBase
    {
        public override void OnInit(int serialId, string uiAssetName, IUIGroup uiGroup, UIViewModelBase viewModel,
            UICtorInfo ctorInfo)
        {
            base.OnInit(serialId, uiAssetName, uiGroup, viewModel, ctorInfo);
            
            BindingSet<TestPage, TestPageViewModel> bindingSet = this.CreateBindingSet<TestPage, TestPageViewModel>();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("text")).For(v => v.text).To(vm => vm.SerialId).OneWay();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("tip")).For(v => v.text).ToExpression(vm => $"tip : {vm.Tips}")
                .OneWay();
            bindingSet.Bind(GetVariable<UISpriteLoadProxy>("icon_spriteloader")).For(v => v.CurLocation).To(vm => vm.SpritePath).OneWay();
            bindingSet.Bind(GetVariable<Image>("image")).For(v => v.enabled).To(vm => vm.ShowSprite)
                .OneWay();
            bindingSet.Build();
            
            var ctor = ctorInfo as TestPageCtorInfo;
            GetViewModel<TestPageViewModel>().Tips = ctor?.Tips;
        }
    }

    public class TestPageViewModel : UIViewModelBase
    {
        private int serialId;
        private string tips;
        private bool showSprite;
        private string spritePath;
        public int SerialId
        {
            get { return serialId; }
            set { Set(ref serialId, value); }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips
        {
            get { return tips; }
            set { Set(ref tips, value); }
        }

        public string SpritePath
        {
            get { return spritePath; }
            set { Set(ref spritePath, value); }
        }

        public bool ShowSprite
        {
            get { return showSprite; }
            set { Set(ref showSprite, value); }
        }

        public async UniTask LoadImage()
        {
            SpritePath = "Assets/ArtAssets/Texture/aioicon.png";
            ShowSprite = true;
        }

        public override void Clear()
        {
            base.Clear();
            spritePath = null;
            serialId = 0;
            tips = null;
            showSprite = false;
        }
    }
}
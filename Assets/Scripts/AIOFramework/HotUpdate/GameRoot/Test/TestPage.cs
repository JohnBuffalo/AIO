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
            bindingSet.Bind(GetVariable<Image>("image")).For(v => v.sprite).To(vm => vm.Sprite).OneWay();
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
        private Sprite sprite;
        private bool showSprite;

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

        public Sprite Sprite
        {
            get { return sprite; }
            set { Set(ref sprite, value); }
        }

        public bool ShowSprite
        {
            get { return showSprite; }
            set { Set(ref showSprite, value); }
        }

        public async UniTask LoadImage()
        {
            Sprite = await LoadAsset<Sprite>("Assets/ArtAssets/Texture/aioicon.png");
            ShowSprite = Sprite != null;
        }

        public override void Clear()
        {
            base.Clear();
            serialId = 0;
            tips = null;
            sprite = null;
            showSprite = false;
        }
    }
}
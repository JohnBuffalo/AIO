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
            ViewModel = viewModel;
            BindingSet<TestPage, TestPageViewModel> bindingSet = this.CreateBindingSet<TestPage, TestPageViewModel>();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("text")).For(v => v.text).To(vm => vm.SerialId).OneWay();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("tip")).For(v => v.text).ToExpression(vm => $"tip : {vm.Tips}")
                .OneWay();
            bindingSet.Bind(GetVariable<Image>("image")).For(v => v.sprite).To(vm => vm.Sprite).OneWay();
            bindingSet.Bind(GetVariable<Image>("image")).For(v => v.enabled).To(vm => vm.ShowSprite)
                .OneWay();
            bindingSet.Build();
        }
    }

    public class TestPageViewModel : UIViewModelBase
    {
        private int m_SerialId;
        private string m_Tips;
        private Sprite m_Sprite;
        private bool m_ShowSprite;

        public int SerialId
        {
            get { return m_SerialId; }
            set { Set(ref m_SerialId, value); }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips
        {
            get { return m_Tips; }
            set { Set(ref m_Tips, value); }
        }

        public Sprite Sprite
        {
            get { return m_Sprite; }
            set { Set(ref m_Sprite, value); }
        }

        public bool ShowSprite
        {
            get { return m_ShowSprite; }
            set { Set(ref m_ShowSprite, value); }
        }

        public async UniTask LoadImage()
        {
            Sprite = await LoadAsset<Sprite>("Assets/ArtAssets/Texture/aioicon.png");
            ShowSprite = Sprite != null;
        }
    }
}
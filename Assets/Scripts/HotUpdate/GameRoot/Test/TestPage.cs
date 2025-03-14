using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using TMPro;
using AIOFramework.UI;

namespace AIOFramework.Runtime
{
    public class TestPage : UIViewBase
    {
        public override void OnInit(int serialId, string uiAssetName, IUIGroup uiGroup, UIViewModelBase viewModel, UICtorInfo ctorInfo)
        {
            base.OnInit(serialId, uiAssetName, uiGroup, viewModel,ctorInfo);
            ViewModel = viewModel;
            BindingSet<TestPage, TestPageViewModel> bindingSet = this.CreateBindingSet<TestPage, TestPageViewModel>();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("text")).For(v => v.text).To(vm => vm.SerialId ).OneWay();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("tip")).For(v => v.text).ToExpression(vm => $"tip : { vm.Tips}").OneWay();
            bindingSet.Build();
        }
    }

    public class TestPageViewModel : UIViewModelBase
    {
        private int m_SerialId;
        private string m_Tips;
        
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
        
    }
}


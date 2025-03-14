using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AIOFramework.UI;

namespace AIOFramework.Runtime
{
    public class MessageBoxView : UIViewBase
    {
        private GameObject panel;
        private TextMeshProUGUI tip;
        private Button button;

        protected override void Awake()
        {
            base.Awake();
            panel = gameObject;
            tip = GetVariable<TextMeshProUGUI>("tips");
            button = GetVariable<Button>("button");
        }

        public void BindContext(MessageBoxViewModel vm)
        {
            this.SetDataContext(vm);
        }

    protected override void Start()
        {
            BindingSet<MessageBoxView, MessageBoxViewModel> bindingSet = this.CreateBindingSet<MessageBoxView, MessageBoxViewModel>();
            bindingSet.Bind(panel).For(v => v.activeSelf).To(vm => vm.Display).OneWay();
            bindingSet.Bind(tip).For(v=>v.text).To(vm => vm.Tip).OneWay();
            bindingSet.Bind(button).For(v => v.onClick).To(vm => vm.OkCommand).CommandParameter(this.GetDataContext);
            bindingSet.Build();
        }
    }
}
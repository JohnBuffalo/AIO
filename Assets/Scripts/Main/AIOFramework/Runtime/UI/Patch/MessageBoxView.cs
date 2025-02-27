using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIOFramework.Runtime
{
    public class MessageBoxView : UIView
    {
        private GameObject panel;
        private TextMeshProUGUI tip;
        private Button button;

        protected override void Awake()
        {
            base.Awake();
            panel = gameObject;
            tip = transform.Find("tips").GetComponent<TextMeshProUGUI>();
            button = transform.Find("Button").GetComponent<Button>();
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
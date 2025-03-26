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
        private GameObject _panel;
        private TextMeshProUGUI _tip;
        private Button _button;

        protected override void Awake()
        {
            base.Awake();
            _panel = gameObject;
            _tip = GetVariable<TextMeshProUGUI>("tips");
            _button = GetVariable<Button>("button");
        }

        public void BindContext(MessageBoxViewModel vm)
        {
            ViewModel = vm;
        }

        protected override void Start()
        {
            BindingSet<MessageBoxView, MessageBoxViewModel> bindingSet =
                this.CreateBindingSet<MessageBoxView, MessageBoxViewModel>();
            bindingSet.Bind(_panel).For(v => v.activeSelf).To(vm => vm.Display).OneWay();
            bindingSet.Bind(_tip).For(v => v.text).To(vm => vm.Tip).OneWay();
            bindingSet.Bind(_button).For(v => v.onClick).To(vm => vm.OkCommand).CommandParameter(this.GetDataContext);
            bindingSet.Build();
        }
        
        protected override void OnDestroy()
        {
            ReferencePool.Release(ViewModel);
            ViewModel = null;
        }
    }
}
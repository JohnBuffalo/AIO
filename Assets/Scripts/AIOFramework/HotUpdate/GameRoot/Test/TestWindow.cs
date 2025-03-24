using System;
using AIOFramework.UI;
using HotUpdate;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using TMPro;
using UnityEngine.UI;

namespace AIOFramework.Runtime
{
    public class TestWindow : UIViewBase
    {
        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            var vm = GetViewModel<TestWindowViewModel>();
            vm.CloseWindowCommand = new SimpleCommand(() =>
            {
                vm.CloseWindowCommand.Enabled = false;
                OnButtonClick(this, null);
                vm.CloseWindowCommand.Enabled = true;
            });
        }

        public override void OnInit(int serialId, string uiAssetName, IUIGroup uiGroup, UIViewModelBase viewModel,
            UICtorInfo ctorInfo)
        {
            base.OnInit(serialId, uiAssetName, uiGroup, viewModel, ctorInfo);
            BindingSet<TestWindow, TestWindowViewModel> bindingSet =
                this.CreateBindingSet<TestWindow, TestWindowViewModel>();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("text")).For(v => v.text).To(vm => vm.SerialId).OneWay();
            bindingSet.Bind(GetVariable<TextMeshProUGUI>("tip")).For(v => v.text).ToExpression(vm => $"tip : {vm.Tips}")
                .OneWay();
            bindingSet.Bind(GetVariable<Button>("button")).For(v => v.onClick).To(vm => vm.CloseWindowCommand).OneWay();
            bindingSet.Build();

            var ctor = ctorInfo as TestWindowCtorInfo;
            GetViewModel<TestWindowViewModel>().Tips = ctor.Tips;
            GetViewModel<TestWindowViewModel>().SerialId = SerialId;
        }

        private void OnButtonClick(object sender, InteractionEventArgs args)
        {
            Game.UI.CloseUI(this);
        }
    }

    public class TestWindowViewModel : UIViewModelBase
    {
        private int _serialId;
        private string _tips;
        private bool _showSprite;
        private Button _button;
        private SimpleCommand _closeWindowCommand;

        public int SerialId
        {
            get { return _serialId; }
            set { Set(ref _serialId, value); }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips
        {
            get { return _tips; }
            set { Set(ref _tips, value); }
        }

        public Button Button
        {
            get { return _button; }
            set { Set(ref _button, value); }
        }

        public SimpleCommand CloseWindowCommand
        {
            get { return _closeWindowCommand; }
            set { Set(ref _closeWindowCommand, value); }
        }

        public override void Clear()
        {
            CloseWindowCommand = null;
            Button = null;
            Tips = null;
            SerialId = 0;
        }
    }
}
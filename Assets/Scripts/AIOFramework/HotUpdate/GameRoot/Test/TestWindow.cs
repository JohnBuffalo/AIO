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
        private int serialId;
        private string tips;
        private bool showSprite;
        private Button button;
        private SimpleCommand closeWindowCommand;

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

        public Button Button
        {
            get { return button; }
            set { Set(ref button, value); }
        }

        public SimpleCommand CloseWindowCommand
        {
            get { return closeWindowCommand; }
            set { Set(ref closeWindowCommand, value); }
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
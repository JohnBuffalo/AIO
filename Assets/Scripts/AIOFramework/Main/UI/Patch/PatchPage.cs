using System;
using System.Collections.Generic;
using AIOFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;

namespace AIOFramework.Runtime
{
    public class PatchPage : UIViewBase
    {
        private Slider _slider;
        private TextMeshProUGUI _verTxt;
        private TextMeshProUGUI _infoTxt;
        private MessageBoxView _messageBoxView;

        protected override void Awake()
        {   
            InitComponents();
        }

        public void BindContext(PatchViewModel vm)
        {
            this.SetDataContext(vm);
            
            var messageBoxViewModel = ReferencePool.Acquire<MessageBoxViewModel>();
            _messageBoxView.BindContext(messageBoxViewModel);
            messageBoxViewModel.Display = false;
            
            BindingSet<PatchPage, PatchViewModel> bindingSet = this.CreateBindingSet<PatchPage, PatchViewModel>();
            bindingSet.Bind(this._slider).For(v => v.value).To(vm => vm.Model.Progress).OneWay();
            bindingSet.Bind(this._verTxt).For(v => v.text).To(vm => vm.Model.Version).OneWay();
            bindingSet.Bind(this._infoTxt).For(v => v.text).To(vm => vm.Model.Info).OneWay();
            bindingSet.Bind().For(v=>v.OnFindHotUpdate).To(vm=>vm.HotUpdateConfirmDialogRequest);
            bindingSet.Bind().For(v => v.OnInitPackageFailed).To(vm => vm.InitPackageFailedDialogRequest);
            bindingSet.Bind().For(v => v.OnHotUpdateFailed).To(vm => vm.HotUpdateFailedDialogRequest);
            bindingSet.Bind().For(v => v.OnHotUpdateFinish).To(vm => vm.HotUpdateFinishRequest);
            bindingSet.Bind().For(v => v.OnSpaceNotEnough).To(vm => vm.SpaceNotEnoughRequest);
            bindingSet.Build();
        }

        private void InitComponents()
        {
            _slider = GetVariable<Slider>("progress");
            _verTxt = GetVariable<TextMeshProUGUI>("version");
            _infoTxt = GetVariable<TextMeshProUGUI>("info");
            _infoTxt.text = "AIO Launch";
            _messageBoxView = GetVariable<MessageBoxView>("messagebox");
        }

        private void OnFindHotUpdate(object sender, InteractionEventArgs args)
        {
            DialogNotification notification = args.Context as DialogNotification;
            var callback = args.Callback;

            if (notification == null)
                return;
            
            ShowMessage(notification, callback);
        }
        
        private void OnInitPackageFailed(object sender, InteractionEventArgs args)
        {
            Notification notification = args.Context as Notification;
            var callback = args.Callback;
            if (notification == null)
                return; 
            ShowMessage(notification, callback);
        }

        private void OnHotUpdateFailed(object sender, InteractionEventArgs args)
        {
            DialogNotification notification = args.Context as DialogNotification;
            var callback = args.Callback;
            if (notification == null) return;
            ShowMessage(notification, callback);
        }

        private void OnHotUpdateFinish(object sender,InteractionEventArgs args)
        {
            Debug.Log("OnHotUpdateFinish Destroy PatchPage");
            Destroy(gameObject);
        }

        private void OnSpaceNotEnough(object sender, InteractionEventArgs args)
        {
            DialogNotification notification = args.Context as DialogNotification;
            var callback = args.Callback;
            if (notification == null) return;
            ShowMessage(notification, callback);
        }

        private void ShowMessage(Notification notifaction, Action callback)
        {
            //打开页面,传递展示信息,传递回调
            var messageBoxViewModel = _messageBoxView.GetDataContext() as MessageBoxViewModel;
            if(messageBoxViewModel == null)
                return;
            
            messageBoxViewModel.Display = true;
            messageBoxViewModel.Tip = notifaction.Message;
            messageBoxViewModel.Title = notifaction.Title;
            messageBoxViewModel.OkCommand = new SimpleCommand(() =>
            {
                callback?.Invoke();
                messageBoxViewModel.Display = false;
            });
        }

        protected override void OnDestroy()
        {
            ReferencePool.Release(ViewModel);
            ViewModel = null;
        }
    }
}
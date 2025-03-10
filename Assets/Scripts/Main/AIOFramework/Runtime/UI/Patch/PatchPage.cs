using System;
using System.Collections.Generic;
using AIOFramework.Event;
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
        private Slider slider;
        private TextMeshProUGUI ver_txt;
        private TextMeshProUGUI info_txt;
        private MessageBoxView messageBoxView;

        protected override void Awake()
        {   
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
            InitComponents();
        }

        public void BindContext(PatchViewModel vm)
        {
            this.SetDataContext(vm);
            
            var messageBoxViewModel = new MessageBoxViewModel();
            messageBoxView.BindContext(messageBoxViewModel);
            messageBoxViewModel.Display = false;
            
            BindingSet<PatchPage, PatchViewModel> bindingSet = this.CreateBindingSet<PatchPage, PatchViewModel>();
            bindingSet.Bind(this.slider).For(v => v.value).To(vm => vm.Model.Progress).OneWay();
            bindingSet.Bind(this.ver_txt).For(v => v.text).To(vm => vm.Model.Version).OneWay();
            bindingSet.Bind(this.info_txt).For(v => v.text).To(vm => vm.Model.Info).OneWay();
            bindingSet.Bind().For(v=>v.OnFindHotUpdate).To(vm=>vm.HotUpdateConfirmDialogRequest);
            bindingSet.Bind().For(v => v.OnInitPackageFailed).To(vm => vm.InitPackageFailedDialogRequest);
            bindingSet.Bind().For(v => v.OnHotUpdateFailed).To(vm => vm.HotUpdateFailedDialogRequest);
            bindingSet.Bind().For(v => v.OnHotUpdateFinish).To(vm => vm.HotUpdateFinishRequest);
            bindingSet.Bind().For(v => v.OnSpaceNotEnough).To(vm => vm.SpaceNotEnoughRequest);
            bindingSet.Build();
        }

        private void InitComponents()
        {
            slider = GetVariable<Slider>("progress");
            ver_txt = GetVariable<TextMeshProUGUI>("version");
            info_txt = GetVariable<TextMeshProUGUI>("info");
            info_txt.text = "AIO Launch";
            messageBoxView = GetVariable<MessageBoxView>("messagebox");
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
            var messageBoxViewModel = messageBoxView.GetDataContext() as MessageBoxViewModel;
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
    }
}
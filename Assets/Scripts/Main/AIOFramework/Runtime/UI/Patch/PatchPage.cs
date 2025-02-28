using System;
using System.Collections.Generic;
using GameFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;

namespace AIOFramework.Runtime
{
    public class PatchPage : UIView
    {
        private Slider slider;
        private TextMeshProUGUI ver_txt;
        private TextMeshProUGUI info_txt;
        private MessageBoxView messageBoxView;
        
        private void Awake()
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
            bindingSet.Build();
        }

        private void InitComponents()
        {
            slider = transform.Find("progress").GetComponent<Slider>();
            ver_txt = transform.Find("version").GetComponent<TextMeshProUGUI>();
            info_txt = transform.Find("info").GetComponent<TextMeshProUGUI>();
            info_txt.text = "AIO Launch";
            
            var _messageBoxObj = transform.Find("messagebox").gameObject;
            messageBoxView = _messageBoxObj.GetComponent<MessageBoxView>();

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
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            PatchViewModel vm = this.GetDataContext() as PatchViewModel;
            vm?.Dispose();
        }
    }
}
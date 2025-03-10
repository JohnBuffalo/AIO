using System;
using AIOFramework.Event;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using PropertyChanged;
using UnityEngine;

namespace AIOFramework.Runtime
{
    [AddINotifyPropertyChangedInterface]
    public class PatchModel
    {
        public string Version
        {
            get;
            set;
        }
        public int TotalFileCount
        {
            get;
            set;
        }
        public int DownloadFileCount
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }
        public float Progress
        {
            get;
            private set;
        }
        
        public void UpdateProgress()
        {
            Progress = (float)DownloadFileCount / TotalFileCount;
        }
    }
    
    public class PatchViewModel : UIViewModelBase
    {
        private PatchModel model;
        
        public PatchModel Model
        {
            get { return model; }
            set { Set(ref model, value, "Model"); }
        }
        
        //打开热更新确认对话框
        public SimpleCommand OpenHotUpdateConfirmDialogCommand { get; set; }
        public InteractionRequest<DialogNotification> HotUpdateConfirmDialogRequest = new InteractionRequest<DialogNotification>();
        //打开热更新失败对话框
        public SimpleCommand OpenHotUpdateFailedDialogCommand { get; set; }
        public InteractionRequest<DialogNotification> HotUpdateFailedDialogRequest = new InteractionRequest<DialogNotification>();
        //提示初始化Package失败
        public SimpleCommand InitPackageFailedCommand { get; set; }
        public InteractionRequest<Notification> InitPackageFailedDialogRequest = new InteractionRequest<Notification>();
        //热更新结束
        public SimpleCommand HotUpdateFinishCommand { get; set; }
        public InteractionRequest<Notification> HotUpdateFinishRequest = new InteractionRequest<Notification>();
        //磁盘空间不足
        public SimpleCommand SpaceNotEnoughCommand { get; set; }
        public InteractionRequest<DialogNotification> SpaceNotEnoughRequest = new InteractionRequest<DialogNotification>();
        
        public PatchViewModel()
        {
            AddListeners();
        }

        public override void Clear()
        {
            base.Clear();
            Model = null;
            RemoveAllListeners();
        }

        private void AddListeners()
        {
            Entrance.Event.Subscribe(PatchStateChangeEventArgs.EventId, OnPatchStateChange);
            Entrance.Event.Subscribe(FindUpdateFilesEventArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Subscribe(PackageVersionEventArgs.EventId, OnPackageVersion);
            Entrance.Event.Subscribe(InitPackageFailedEventArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Subscribe(DownloadFilesFailedEventArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Subscribe(DownloadProgressEventArgs.EventID, OnDownloadProgress);
            Entrance.Event.Subscribe(HotUpdateFinishEventArgs.EventID, OnHotUpdateFinish);
            Entrance.Event.Subscribe(SpaceNotEnoughEventArgs.EventId, OnSpaceNotEnough);
        }

        private void RemoveAllListeners()
        {
            Log.Info("PatchPage RemoveAllListeners");
            Entrance.Event.Unsubscribe(PatchStateChangeEventArgs.EventId, OnPatchStateChange);
            Entrance.Event.Unsubscribe(FindUpdateFilesEventArgs.EventId, OnFindUpdateFiles);
            Entrance.Event.Unsubscribe(PackageVersionEventArgs.EventId, OnPackageVersion);
            Entrance.Event.Unsubscribe(InitPackageFailedEventArgs.EventId, OnInitPackageFailed);
            Entrance.Event.Unsubscribe(DownloadFilesFailedEventArgs.EventId, OnDownloadFilesFailed);
            Entrance.Event.Unsubscribe(DownloadProgressEventArgs.EventID, OnDownloadProgress);
            Entrance.Event.Unsubscribe(HotUpdateFinishEventArgs.EventID, OnHotUpdateFinish);
            Entrance.Event.Unsubscribe(SpaceNotEnoughEventArgs.EventId, OnSpaceNotEnough);
        }

        void OnPatchStateChange(object sender, GameEventArgs gameEventArgs)
        {
            PatchStateChangeEventArgs args = gameEventArgs as PatchStateChangeEventArgs;
            Model.Info = args.Tips;
        }

        void OnPackageVersion(object sender, GameEventArgs gameEventArgs)
        {
            PackageVersionEventArgs args = gameEventArgs as PackageVersionEventArgs;
            Model.Version = args.PackageVersion;
        }

        void OnDownloadProgress(object sender, GameEventArgs gameEventArgs)
        {
            DownloadProgressEventArgs args = gameEventArgs as DownloadProgressEventArgs;
            Model.TotalFileCount = args.TotalDownloadCount;
            Model.DownloadFileCount = args.CurrentDownloadCount;
            Model.UpdateProgress();
            string currentSizeMB = (args.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (args.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            Model.Info = $"{Model.DownloadFileCount}/{Model.TotalFileCount}\t{currentSizeMB}MB/{totalSizeMB}MB";
        }

        void OnFindUpdateFiles(object sender, GameEventArgs gameEventArgs)
        {
            FindUpdateFilesEventArgs args = gameEventArgs as FindUpdateFilesEventArgs;
            float sizeMB = args.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            OpenHotUpdateConfirmDialogCommand = new SimpleCommand(() =>
            {
                OpenHotUpdateConfirmDialogCommand.Enabled = false;
                DialogNotification notification = new DialogNotification("Find HotUpdate", $"Update now? \n Total count = {args.TotalCount}, Total size = {totalSizeMB}MB","Yes","No");
                Action<DialogNotification> callback = n =>
                {
                    OpenHotUpdateConfirmDialogCommand.Enabled = true;
                    Entrance.Event.Fire(this, BeginDownloadUpdateFilesEventArgs.Create());
                };
                HotUpdateConfirmDialogRequest.Raise(notification, callback);
            });
            OpenHotUpdateConfirmDialogCommand.Execute(null);
        }

        void OnInitPackageFailed(object sender, GameEventArgs gameEventArgs)
        {
            InitPackageFailedCommand = new SimpleCommand(() =>
            {
                InitPackageFailedCommand.Enabled = false;
                Notification notification = new Notification("Init Package Failed", 
                    "Init Package Failed, please check the network and try again.");
                Action<Notification> callback = n =>
                {
                    InitPackageFailedCommand.Enabled = true;
                    Application.Quit();
                };
                InitPackageFailedDialogRequest.Raise(notification, callback);
            });
            OpenHotUpdateConfirmDialogCommand.Execute(null);
        }

        void OnDownloadFilesFailed(object sender, GameEventArgs gameEventArgs)
        {
            DownloadFilesFailedEventArgs args = gameEventArgs as DownloadFilesFailedEventArgs;
            OpenHotUpdateFailedDialogCommand = new SimpleCommand(() =>
            {
                OpenHotUpdateFailedDialogCommand.Enabled = false;
                DialogNotification notification = new DialogNotification("Download Failed",
                    $"Download failed, please check the network and try again. \n {args.Error}", "OK");
                Action<DialogNotification> callback = n =>
                {
                    OpenHotUpdateFailedDialogCommand.Enabled = true;
                    Application.Quit();
                };
                HotUpdateFailedDialogRequest.Raise(notification, callback);
            });
            OpenHotUpdateFailedDialogCommand.Execute(null);
        }

        void OnHotUpdateFinish(object sender, GameEventArgs gameEventArgs)
        {
            HotUpdateFinishCommand = new SimpleCommand(() =>
            {
                HotUpdateFinishRequest.Raise(null,null);
            });
            HotUpdateFinishCommand.Execute(null);
        }

        void OnSpaceNotEnough(object sender, GameEventArgs gameEventArgs)
        {
            SpaceNotEnoughEventArgs args = gameEventArgs as SpaceNotEnoughEventArgs;
            string needSpace = (args.NeedSpace / 1048576f).ToString("f1");
            string freeSpace = (args.FreeSpace / 1048576f).ToString("f1");
            
            SpaceNotEnoughCommand = new SimpleCommand(() =>
            {
                SpaceNotEnoughCommand.Enabled = false;
                DialogNotification notification = new DialogNotification("Space Not Enough",
                    $"Space not enough : need: {needSpace} , have: {freeSpace}", "OK");
                
                Action<DialogNotification> callback = n =>
                {
                    SpaceNotEnoughCommand.Enabled = true;
                    Application.Quit();
                };
                
                SpaceNotEnoughRequest.Raise(notification, callback);
            });
            SpaceNotEnoughCommand.Execute(null);
        }
    }
}
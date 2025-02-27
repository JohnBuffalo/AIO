using System;
using System.ComponentModel;
using Loxodon.Framework.ViewModels;
using GameFramework.Event;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using UnityEngine;

namespace AIOFramework.Runtime
{
    public class PatchModel : INotifyPropertyChanged
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
            set;
        }
        
        public void UpdateProgress()
        {
            Progress = (float)DownloadFileCount / TotalFileCount;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
    
    public class PatchViewModel : ViewModelBase
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
        
        public PatchViewModel(PatchModel model)
        {
            Model = model;
            AddListeners();
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
            Log.Error("PackageVersion:" + Model.Version);
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
            float sizeMB = args.TotalCount / 1048576f;
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
            Action callback = () =>
            {
                Log.Info("Application Quit");
                Application.Quit();
            };
            // ShowMessageBox("Failed to initialize the package, please check the network and try again.", callback);
        }

        void OnDownloadFilesFailed(object sender, GameEventArgs gameEventArgs)
        {
            DownloadFilesFailedEventArgs args = gameEventArgs as DownloadFilesFailedEventArgs;
            Action callback = () => { Application.Quit(); };
            // ShowMessageBox($"Download failed, please check the network and try again. \n {args.Error}", callback);
        }
    }
}
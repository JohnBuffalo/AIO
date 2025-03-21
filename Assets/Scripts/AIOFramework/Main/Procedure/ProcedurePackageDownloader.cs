using System;
using System.IO;
using Cysharp.Threading.Tasks;
using AIOFramework.Event;
using AIOFramework.Procedure;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    public class ProcedurePackageDownloader : ProcedureBase
    {
        private ProcedureOwner procedureOwner;

        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            AddListeners();
            this.procedureOwner = procedureOwner;
            Entrance.Event.Fire(this, PatchStateChangeEventArgs.Create("CreatePackageDownloader"));
            CreateDownloader(procedureOwner).Forget();
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            RemoveListeners();
        }

        private void AddListeners()
        {
            Entrance.Event.Subscribe(BeginDownloadUpdateFilesEventArgs.EventId, OnBeginDownloadUpdateFiles);
        }

        private void RemoveListeners()
        {
            Entrance.Event.Unsubscribe(BeginDownloadUpdateFilesEventArgs.EventId, OnBeginDownloadUpdateFiles);
        }

        private void OnBeginDownloadUpdateFiles(object sender, GameEventArgs e)
        {
            ChangeState<ProcedureDownloadPackageFiles>(procedureOwner);
        }

        private async UniTask CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            var packageName = procedureOwner.GetData<VarString>("PackageName");
            var package = Entrance.Resource.GetAssetsPackage(packageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            procedureOwner.SetData<VarDownloader>("Downloader", downloader);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("No file need download, procedure done");
                ChangeState<ProcedureUpdateDone>(procedureOwner);
            }
            else
            {
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                var rootPath = YooAssetSettingsData.GetYooDefaultCacheRoot();
                long freeSpace = DiskSpace(rootPath);
                Log.Info($"Need Download File Count {totalDownloadCount}, total Size {totalDownloadBytes / (1024f * 1024f):F2} MB");
                if (freeSpace > totalDownloadBytes)
                {
                    Entrance.Event.Fire(this, FindUpdateFilesEventArgs.Create(totalDownloadCount, totalDownloadBytes));
                }
                else
                {
                    Entrance.Event.Fire(this, SpaceNotEnoughEventArgs.Create(totalDownloadBytes, freeSpace));
                }
            }
        }
        
        private long DiskSpace(string path)
        {
            long freeSpace = 0L;
            try
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                // Windows 平台
                var driveInfo = new DriveInfo(Path.GetPathRoot(path));
                freeSpace = driveInfo.AvailableFreeSpace;
                Debug.Log($"Windows可用空间: {freeSpace / (1024f * 1024f * 1024f):F1} GB");

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                // macOS 平台
                var driveInfo = new DriveInfo(Path.GetPathRoot(path));
                freeSpace = driveInfo.AvailableFreeSpace;
                Debug.Log($"macOS可用空间: {freeSpace / (1024f * 1024f * 1024f):F1} GB");

#elif UNITY_ANDROID
                // Android 平台
                using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
                using (AndroidJavaObject dataDirectory = environment.CallStatic<AndroidJavaObject>("getDataDirectory"))
                using (AndroidJavaObject stat =
 new AndroidJavaObject("android.os.StatFs", dataDirectory.Call<string>("getPath")))
                {
                    long blockSize = stat.Call<long>("getBlockSizeLong");
                    long availableBlocks = stat.Call<long>("getAvailableBlocksLong");
                    freeSpace = availableBlocks * blockSize;
                    Debug.Log($"Android可用空间: {freeSpace / (1024f * 1024f * 1024f):F1} GB");
                }

#elif UNITY_IOS
                // iOS 平台
                string docPath = path ?? Application.persistentDataPath;
                Dictionary<string, string> fileSystemAttrs =
 (Dictionary<string, string>)Directory.GetParent(docPath).GetDirectoryInfo().GetFileSystemAttributes();
                if (fileSystemAttrs.ContainsKey("NSFileSystemFreeSize"))
                {
                    freeSpace = long.Parse(fileSystemAttrs["NSFileSystemFreeSize"]);
                    Debug.Log($"iOS可用空间: {freeSpace / (1024f * 1024f * 1024f):F1} GB");
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"检查磁盘空间时发生错误: {e.Message}");
            }
            return freeSpace;
        }
    }
}
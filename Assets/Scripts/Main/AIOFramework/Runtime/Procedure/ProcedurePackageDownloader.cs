using System;
using System.IO;
using Cysharp.Threading.Tasks;
using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using Object = UnityEngine.Object;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    public class ProcedurePackageDownloader : ProcedureBase
    {
        private ProcedureOwner procedureOwner;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            AddListeners();
            this.procedureOwner = procedureOwner;
            Entrance.Event.Fire(this, PatchStateChangeEventArgs.Create("CreatePackageDownloader"));
            CreateDownloader(procedureOwner).Forget();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
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
            await UniTask.WaitForSeconds(0.5f);

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
                Log.Info($"Need Download File Count {totalDownloadCount}, total Bytes {totalDownloadBytes}");

                await OpenPatchPage();

                Entrance.Event.Fire(this, FindUpdateFilesEventArgs.Create(totalDownloadCount, totalDownloadBytes));
                // CheckDiskSpace(totalDownloadBytes); 
            }
        }

        private async UniTask OpenPatchPage()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("Asset/PatchPage");
            var prefab = await request.ToUniTask();

            if (prefab == null)
            {
                Log.Error("Load PatchPage Failed");
                return;
            }

            var canvasRoot = GameObject.Find("Canvas").transform;
            
            GameObject patchPage = Object.Instantiate(prefab,canvasRoot) as GameObject;

            PatchPage patchView = patchPage.GetComponent<PatchPage>();
            PatchViewModel patchViewModel = new PatchViewModel(new PatchModel());
            patchView.BindContext(patchViewModel);
            
            patchViewModel.Model.Version = procedureOwner.GetData<VarString>("PackageVersion");
        }

        private void DiskSpace(string path)
        {
            try
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                // Windows 平台
                var driveInfo = new DriveInfo(Path.GetPathRoot(path));
                long freeSpace = driveInfo.AvailableFreeSpace;
                Debug.Log($"Windows可用空间: {freeSpace / (1024f * 1024f * 1024f):F2} GB");

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                // macOS 平台
                var driveInfo = new DriveInfo(Path.GetPathRoot(path));
                long freeSpace = driveInfo.AvailableFreeSpace;
                Debug.Log($"macOS可用空间: {freeSpace / (1024f * 1024f * 1024f):F2} GB");

#elif UNITY_ANDROID
                // Android 平台
                using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
                using (AndroidJavaObject dataDirectory = environment.CallStatic<AndroidJavaObject>("getDataDirectory"))
                using (AndroidJavaObject stat =
 new AndroidJavaObject("android.os.StatFs", dataDirectory.Call<string>("getPath")))
                {
                    long blockSize = stat.Call<long>("getBlockSizeLong");
                    long availableBlocks = stat.Call<long>("getAvailableBlocksLong");
                    long freeSpace = availableBlocks * blockSize;
                    Debug.Log($"Android可用空间: {freeSpace / (1024f * 1024f * 1024f):F2} GB");
                }

#elif UNITY_IOS
                // iOS 平台
                string docPath = path ?? Application.persistentDataPath;
                Dictionary<string, string> fileSystemAttrs =
 (Dictionary<string, string>)Directory.GetParent(docPath).GetDirectoryInfo().GetFileSystemAttributes();
                if (fileSystemAttrs.ContainsKey("NSFileSystemFreeSize"))
                {
                    long freeSpace = long.Parse(fileSystemAttrs["NSFileSystemFreeSize"]);
                    Debug.Log($"iOS可用空间: {freeSpace / (1024f * 1024f * 1024f):F2} GB");
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"检查磁盘空间时发生错误: {e.Message}");
            }
        }
    }
}
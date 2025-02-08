using GameFramework;
using YooAsset;
using System;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace AIO.Framework.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public partial class AssetManager : GameFrameworkModule
    {
        public string DefaultPackageName { get; set; } = "DefaultPackage";
        public EFileVerifyLevel VerifyLevel { get; set; }
        public long Milliseconds { get; set; } = 30;
        private EPlayMode _playMode;
        public EPlayMode PlayMode { private set; get; }


        public void Initialize()
        {
            Log.Info($"PlayMode:{PlayMode}");
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);
        }

        public UniTask<bool> InitPackageAsync(string packageName, string hostServerURL, string fallbackHostServerURL,
            bool isDefaultPackage = false)
        {
            var taskCompletionSource = new UniTaskCompletionSource<bool>();
            var resourcePackage = YooAssets.TryGetPackage(packageName);
            if (resourcePackage == null)
            {
                resourcePackage = YooAssets.CreatePackage(packageName);
                if (isDefaultPackage)
                {
                    // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
                    YooAssets.SetDefaultPackage(resourcePackage);
                }
            }
        }
        
        
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }

        internal override void Shutdown()
        {
            throw new System.NotImplementedException();
        }
    }
}
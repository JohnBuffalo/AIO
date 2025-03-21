using YooAsset;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using AIOFramework.Runtime;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace AIOFramework.Resource
{
    public partial class ResourceComponent : GameFrameworkComponent, IAssetManager
    {
        private Dictionary<string, List<HandleBase>> handles = new Dictionary<string, List<HandleBase>>();

        private ResourcePackage resourcePackage;

        private ResourcePackage ResourcePackage
        {
            get
            {
                if (resourcePackage == null)
                {
                    resourcePackage = YooAssets.GetPackage(PackageName);
                }

                return resourcePackage;
            }
        }

        public void Initialize()
        {
            Log.Info($"Initialize PlayMode:{PlayMode} , PackageName:{PackageName}");
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(TimeSlice);
        }

        /// <summary>
        /// 创建资源包
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        /// <returns></returns>
        public ResourcePackage CreateAssetsPackage(string packageName)
        {
            return YooAssets.CreatePackage(packageName);
        }

        /// <summary>
        /// 设置默认资源包
        /// </summary>
        /// <param name="resourcePackage">资源信息</param>
        /// <returns></returns>
        public void SetDefaultAssetsPackage(ResourcePackage resourcePackage)
        {
            YooAssets.SetDefaultPackage(resourcePackage);
        }

        /// <summary>
        /// 获取资源包
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        /// <returns></returns>
        public ResourcePackage GetAssetsPackage(string packageName)
        {
            return YooAssets.TryGetPackage(packageName);
        }

        /// <summary>
        /// 是否需要下载
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <returns></returns>
        public bool IsNeedDownload(AssetInfo assetInfo)
        {
            return YooAssets.IsNeedDownloadFromRemote(assetInfo);
        }

        /// <summary>
        /// 是否需要下载
        /// </summary>
        /// <param name="path">资源地址</param>
        /// <returns></returns>
        public bool IsNeedDownload(string path)
        {
            return YooAssets.IsNeedDownloadFromRemote(path);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="assetTags">资源标签列表</param>
        /// <returns></returns>
        public AssetInfo[] GetAssetInfos(string[] assetTags)
        {
            return YooAssets.GetAssetInfos(assetTags);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="assetTag">资源标签</param>
        /// <returns></returns>
        public AssetInfo[] GetAssetInfos(string assetTag)
        {
            return YooAssets.GetAssetInfos(assetTag);
        }

        private bool TryReleaseHandle(string assetPath)
        {
            var handle = TryPopHandle<HandleBase>(assetPath);
            if (handle == null) return false;
            handle.Release();
            return true;
        }

        private bool TryReleaseHandle(HandleBase handle)
        {
            if (TryPopHandle(handle))
            {
                handle.Release();
                return true;
            }

            return false;
        }

        public T TryPopHandle<T>(string assetPath) where T : HandleBase
        {
            if (handles.TryGetValue(assetPath, out var handleList))
            {
                T handle = handleList[^1] as T;
                if (handle == null) return null;
                handleList.RemoveAt(handleList.Count - 1);
                return handle;
            }

            return null;
        }

        public bool TryPopHandle(HandleBase handle)
        {
            if (handles.TryGetValue(handle.Provider.MainAssetInfo.AssetPath, out var handleList))
            {
                for (int i = handleList.Count - 1; i >= 0; i--)
                {
                    if (handleList[i] == handle)
                    {
                        handleList.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public void RecordHandle<T>(string assetPath, T handle) where T : HandleBase
        {
            if (!handles.TryGetValue(assetPath, out var handleList))
            {
                handleList = new List<HandleBase>();
                handles.Add(assetPath, handleList);
            }

            handleList.Add(handle);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        public AssetInfo GetAssetInfo(string path)
        {
            return YooAssets.GetAssetInfo(path);
        }

        /// <summary>
        /// 检查指定的资源路径是否有效。
        /// </summary>
        /// <param name="path">要检查的资源路径。</param>
        /// <returns>如果资源路径有效，则返回 true；否则返回 false。</returns>
        public bool HasAssetPath(string path)
        {
            return YooAssets.CheckLocationValid(path);
        }

        #region 资源卸载

        /// <summary>
        /// 通过Handle卸载资源
        /// </summary>
        /// <param name="handle"></param>
        public void UnloadAsset(HandleBase handle)
        {
            var assetPath = handle.Provider.MainAssetInfo.AssetPath;
            Log.Info($"Unload Asset:{assetPath}");
            if (TryReleaseHandle(handle))
            {
                ResourcePackage.TryUnloadUnusedAsset(assetPath);
            }
        }

        public void UnloadAsset(string packageName, HandleBase handle)
        {
            var assetPath = handle.Provider.MainAssetInfo.AssetPath;
            if (TryReleaseHandle(handle))
            {
                var package = YooAssets.GetPackage(packageName);
                package?.TryUnloadUnusedAsset(assetPath);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        public void UnloadAsset(string assetPath)
        {
            if (TryReleaseHandle(assetPath))
            {
                ResourcePackage.TryUnloadUnusedAsset(assetPath);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        /// <param name="assetPath">资源路径</param>
        public void UnloadAsset(string packageName, string assetPath)
        {
            if (TryReleaseHandle(assetPath))
            {
                var package = YooAssets.GetPackage(packageName);
                package?.TryUnloadUnusedAsset(assetPath);
            }
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        public void UnloadAllAssetsAsync(string packageName, UnloadAllAssetsOptions options)
        {
            handles.Clear();
            var package = YooAssets.GetPackage(packageName);
            package.UnloadAllAssetsAsync(options);
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public void UnloadAllAssetsAsync(UnloadAllAssetsOptions options)
        {
            handles.Clear();
            ResourcePackage.UnloadAllAssetsAsync(options);
        }

        /// <summary>
        /// 卸载无用资源, 尽量不用
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        public void UnloadUnusedAssetsAsync(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssetsAsync();
        }

        /// <summary>
        /// 卸载无用资源, 尽量不用
        /// </summary>
        public void UnloadUnusedAssetsAsync()
        {
            ResourcePackage.UnloadUnusedAssetsAsync();
        }

        /// <summary>
        /// 销毁实例对象
        /// </summary>
        /// <param name="obj"></param>
        public void DestroyInstance(UnityEngine.Object obj)
        {
            Object.Destroy(obj);
        }

        #endregion

        #region 异步加载资源

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<(T, AssetHandle)> LoadAssetAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object
        {
            AssetHandle handle = ResourcePackage.LoadAssetAsync(assetInfo, priority);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed)
            {
                RecordHandle<AssetHandle>(assetInfo.AssetPath, handle);
                var asset = handle.AssetObject as T;
                return (asset, handle);
            }
            else
            {
                Log.Error("Failed to LoadAssetAsync: " + handle.LastError);
                return (null, null);
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async UniTask<(T, AssetHandle)> LoadAssetAsync<T>(string path, uint priority = 0)
            where T : UnityEngine.Object
        {
            AssetHandle handle = ResourcePackage.LoadAssetAsync<T>(path, priority);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed)
            {
                RecordHandle<AssetHandle>(path, handle);
                var asset = handle.AssetObject as T;
                return (asset, handle);
            }
            else
            {
                Log.Error("Failed to LoadAssetAsync: " + handle.LastError);
                return (null, null);
            }
        }

        /// <summary>
        /// 异步加载bundle中的全部资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<(IReadOnlyList<T>, AllAssetsHandle)> LoadAllAssetsAsync<T>(string path, uint priority = 0)
            where T : UnityEngine.Object
        {
            AllAssetsHandle handle = ResourcePackage.LoadAllAssetsAsync<T>(path, priority);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed)
            {
                var assets = handle.AllAssetObjects as IReadOnlyList<T>;
                RecordHandle<AllAssetsHandle>(path, handle);
                return (assets, handle);
            }
            else
            {
                Log.Error("Failed to LoadAllAssetsAsync: " + handle.LastError);
                return (null, null);
            }
        }

        /// <summary>
        /// 异步加载bundle中的全部资源
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <param name="priority"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THandle"></typeparam>
        /// <returns></returns>
        public async UniTask<(IReadOnlyList<T>,AllAssetsHandle)> LoadAllAssetsAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object
        {
            AllAssetsHandle handle = ResourcePackage.LoadAllAssetsAsync<T>(assetInfo.AssetPath, priority);
            await handle.ToUniTask();
            if (handle.Status == EOperationStatus.Succeed)
            {
                var asset = handle.AllAssetObjects as IReadOnlyList<T>;
                RecordHandle<AllAssetsHandle>(assetInfo.AssetPath, handle);
                return (asset, handle);
            }
            else
            {
                Log.Error("Failed to LoadAllAssetsAsync: " + handle.LastError);
                return (null,null);
            }
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <typeparam name="THandle"></typeparam>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="priority">加载优先级，默认为0</param>
        /// <returns>加载成功返回子资源对象列表，失败返回null</returns>
        public async UniTask<(IReadOnlyList<T>,SubAssetsHandle)> LoadSubAssetsAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object
        {
            SubAssetsHandle handle = ResourcePackage.LoadSubAssetsAsync<T>(assetInfo.AssetPath, priority);
            // 等待加载完成
            await handle.ToUniTask();
            // 检查加载状态
            if (handle.Status == EOperationStatus.Succeed)
            {
                var asset = handle.SubAssetObjects as IReadOnlyList<T>;
                RecordHandle<SubAssetsHandle>(assetInfo.AssetPath, handle);
                return (asset,handle);
            }
            else
            {
                // 如果加载失败，记录错误日志并返回null
                Log.Error("Failed to LoadSubAssetsAsync: " + handle.LastError);
                return (null,null);
            }
        }

        /// <summary>
        /// 异步加载原生文件数据
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="priority">加载优先级，默认为0</param>
        /// <returns>加载成功返回原始文件数据的字节数组，失败返回null</returns>
        public async UniTask<(byte[],RawFileHandle)> LoadRawFileAsync(string path, uint priority = 0)
        {
            RawFileHandle handle = ResourcePackage.LoadRawFileAsync(path, priority);
            await handle.ToUniTask();
            if (handle.Status == EOperationStatus.Succeed)
            {
                var asset = handle.GetRawFileData();
                RecordHandle<RawFileHandle>(path, handle);
                return (asset,handle);
            }
            else
            {
                Log.Error("Failed to LoadRawFileAsync: " + handle.LastError);
                return (null,null);
            }
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="path">场景路径</param>
        /// <param name="mode">加载场景的模式，默认为Single</param>
        /// <param name="activateOnLoad">加载完成后是否激活场景，默认为true</param>
        /// <param name="priority">加载优先级，默认为0</param>
        /// <returns>加载成功返回加载的场景，失败返回null</returns>
        public async UniTask<(Scene,SceneHandle)> LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single,
            bool activateOnLoad = true, uint priority = 0) 
        {
            SceneHandle handle =
                ResourcePackage.LoadSceneAsync(path, mode, LocalPhysicsMode.None, activateOnLoad == false, priority);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed)
            {
                var scene = handle.SceneObject;
                RecordHandle<SceneHandle>(path, handle);
                return (scene,handle);
            }
            else
            {
                Log.Error("Failed to LoadSceneAsync: " + handle.LastError);
                return (default,null);
            }
        }


        /// <summary>
        /// 异步实例化一个Unity对象
        /// </summary>
        /// <typeparam name="T">要实例化的对象类型，必须是UnityEngine.Object的子类</typeparam>
        /// <param name="location">资源的路径或名称</param>
        /// <param name="parent">父级Transform，如果为null，则实例化的对象将没有父级</param>
        /// <param name="position">实例化对象的初始位置，如果未指定，则使用默认值</param>
        /// <param name="rotation">实例化对象的初始旋转，如果未指定，则使用默认值</param>
        /// <returns>实例化后的对象</returns>
        public async UniTask<(T,AssetHandle)> InstantiateAsync<T>(string location, Transform parent = null,
            Vector3 position = default, Quaternion rotation = default) where T : UnityEngine.Object
        {
            (T,AssetHandle) result = await LoadAssetAsync<T>(location);
            T instance = Object.Instantiate(result.Item1, position, rotation, parent);

            return (instance,result.Item2);
        }

        #endregion
    }
}
﻿/// 注释通过AI生成

using System.Collections.Generic;
using YooAsset;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace AIOFramework.Resource
{
    /// <summary>
    /// 资源管理器接口，定义了资源管理的基本操作。
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// 资源包
        /// </summary>
        ResourcePackage ResourcePackage { get; }

        /// <summary>
        /// 初始化资源管理器。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 创建一个新的资源包。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        /// <returns>创建的资源包对象。</returns>
        ResourcePackage CreateAssetsPackage(string packageName);

        /// <summary>
        /// 设置默认的资源包。
        /// </summary>
        /// <param name="resourcePackage">要设置为默认的资源包对象。</param>
        void SetDefaultAssetsPackage(ResourcePackage resourcePackage);

        /// <summary>
        /// 获取指定名称的资源包。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        /// <returns>获取到的资源包对象，如果不存在则返回null。</returns>
        ResourcePackage GetAssetsPackage(string packageName);

        /// <summary>
        /// 异步初始化资源包。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        /// <param name="hostServerURL">主服务器URL。</param>
        /// <param name="fallbackHostServerURL">备用服务器URL。</param>
        /// <param name="isDefaultPackage">是否设置为默认资源包。</param>
        /// <returns>初始化是否成功的任务。</returns>
        UniTask<bool> InitPackageAsync(string packageName, string hostServerURL,
            string fallbackHostServerURL, bool isDefaultPackage = false);

        /// <summary>
        /// 检查指定的资源是否需要下载。
        /// </summary>
        /// <param name="assetInfo">资源信息对象。</param>
        /// <returns>如果需要下载则返回true，否则返回false。</returns>
        bool IsNeedDownload(AssetInfo assetInfo);

        /// <summary>
        /// 检查指定路径的资源是否需要下载。
        /// </summary>
        /// <param name="path">资源路径。</param>
        /// <returns>如果需要下载则返回true，否则返回false。</returns>
        bool IsNeedDownload(string path);

        /// <summary>
        /// 根据资源标签获取资源信息数组。
        /// </summary>
        /// <param name="assetTags">资源标签数组。</param>
        /// <returns>获取到的资源信息数组。</returns>
        AssetInfo[] GetAssetInfos(string[] assetTags);

        /// <summary>
        /// 根据资源标签获取资源信息数组。
        /// </summary>
        /// <param name="assetTag">资源标签。</param>
        /// <returns>获取到的资源信息数组。</returns>
        AssetInfo[] GetAssetInfos(string assetTag);

        T TryPopHandle<T>(string assetPath) where T : HandleBase;
        bool TryPopHandle(HandleBase handle);
        void RecordHandle<T>(string assetPath, T handle) where T : HandleBase;

        /// <summary>
        /// 卸载指定路径的资源。
        /// </summary>
        /// <param name="assetPath">资源路径。</param>
        void UnloadAsset(string assetPath);

        void UnloadAsset(HandleBase handle);

        /// <summary>
        /// 卸载指定包中指定路径的资源。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        /// <param name="assetPath">资源路径。</param>
        void UnloadAsset(string packageName, string assetPath);

        void UnloadAsset(string packageName, HandleBase handle);

        /// <summary>
        /// 异步卸载指定包中的所有资源。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        UniTask UnloadAllAssetsAsync(string packageName, UnloadAllAssetsOptions options = null);

        /// <summary>
        /// 异步卸载所有资源。
        /// </summary>
        UniTask UnloadAllAssetsAsync(UnloadAllAssetsOptions options = null);

        /// <summary>
        /// 异步卸载未使用的资源。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        UniTask UnloadUnusedAssetsAsync(string packageName);

        /// <summary>
        /// 异步卸载未使用的资源。
        /// </summary>
        UniTask UnloadUnusedAssetsAsync();

        /// <summary>
        /// 销毁指定的Unity对象。
        /// </summary>
        /// <param name="obj">要销毁的Unity对象。</param>
        void DestroyInstance(UnityEngine.Object obj);

        /// <summary>
        /// 异步加载指定资源信息的资源。
        /// </summary>
        /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
        /// <param name="assetInfo">资源信息对象。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的资源对象。</returns>
        UniTask<(T, AssetHandle)> LoadAssetAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载指定路径的资源。
        /// </summary>
        /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
        /// <param name="path">资源路径。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的资源对象。</returns>
        UniTask<(T, AssetHandle)> LoadAssetAsync<T>(string path, uint priority = 0)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载指定路径的所有资源。
        /// </summary>
        /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
        /// <param name="path">资源路径。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的资源列表。</returns>
        UniTask<(IReadOnlyList<T>,AllAssetsHandle)> LoadAllAssetsAsync<T>(string path, uint priority = 0)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载指定资源信息的所有资源。
        /// </summary>
        /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
        /// <typeparam name="THandle">HandleBase</typeparam>
        /// <param name="assetInfo">资源信息对象。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的资源列表。</returns>
        UniTask<(IReadOnlyList<T>,AllAssetsHandle)> LoadAllAssetsAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载指定资源信息的子资源。
        /// </summary>
        /// <typeparam name="T">资源类型，必须是UnityEngine.Object的子类。</typeparam>
        /// <param name="assetInfo">资源信息对象。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的子资源列表。</returns>
        UniTask<(IReadOnlyList<T>,SubAssetsHandle)> LoadSubAssetsAsync<T>(AssetInfo assetInfo, uint priority = 0)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载指定路径的原始文件。
        /// </summary>
        /// <param name="path">文件路径。</param>
        /// <param name="priority">加载优先级。</param>
        /// <returns>加载完成的原始文件。</returns>
        UniTask<(byte[], RawFileHandle)> LoadRawFileAsync(string path, uint priority = 0);
        
        /// <summary>
        /// 异步实例化对象
        /// </summary>
        /// <typeparam name="T">要实例化的对象类型，必须是UnityEngine.Object的子类</typeparam>
        /// <param name="location">对象的资源路径</param>
        /// <param name="parent">父级Transform，如果为null，则实例化的对象将没有父级</param>
        /// <param name="position">实例化对象的初始位置，如果未指定，则使用默认值</param>
        /// <param name="rotation">实例化对象的初始旋转，如果未指定，则使用默认值</param>
        /// <returns>实例化后的对象</returns>
        UniTask<(T,AssetHandle)> InstantiateAsync<T>(string location, Transform parent = null,
            Vector3 position = default, Quaternion rotation = default) where T : UnityEngine.Object;
    }
}
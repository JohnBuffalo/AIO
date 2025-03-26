
using System;
using System.Collections.Generic;
using AIOFramework.Resource;
using Cysharp.Threading.Tasks;
using AIOFramework.Event;
using YooAsset;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 场景管理器接口。
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// 加载场景成功事件。
        /// </summary>
        event EventHandler<LoadSceneSuccessEventArgs> LoadSceneSuccess;

        /// <summary>
        /// 加载场景失败事件。
        /// </summary>
        event EventHandler<LoadSceneFailureEventArgs> LoadSceneFailure;


        /// <summary>
        /// 卸载场景成功事件。
        /// </summary>
        event EventHandler<UnloadSceneSuccessEventArgs> UnloadSceneSuccess;

        /// <summary>
        /// 卸载场景失败事件。
        /// </summary>
        event EventHandler<UnloadSceneFailureEventArgs> UnloadSceneFailure;

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resourceManager">资源管理器。</param>
        void SetResourceManager(IAssetManager resourceManager);

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        bool SceneIsLoaded(string location);

        /// <summary>
        /// 获取已加载场景的资源句柄。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>已加载场景的资源句柄。</returns>
        SceneHandle GetLoadedSceneHandle(string location);

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的资源名称。</returns>
        string[] GetLoadedScenes();

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的资源名称。</param>
        void GetLoadedScenes(List<string> results);

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        bool SceneIsLoading(string location);

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <returns>正在加载场景的资源名称。</returns>
        string[] GetLoadingScenes();

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <param name="results">正在加载场景的资源名称。</param>
        void GetLoadingScenes(List<string> results);

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        bool SceneIsUnloading(string location);

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的资源名称。</returns>
        string[] GetUnloadingScenes();

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称。</param>
        void GetUnloadingScenes(List<string> results);


        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        UniTask<SceneHandle> LoadSceneAsync(string location);

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode);

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        /// <param name="activeOnLoad">true=加载完场景后直接激活.false=加载到90%时挂起</param>
        UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode, bool activeOnLoad);

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        /// <param name="activeOnLoad">true=加载完场景后直接激活.false=加载到90%时挂起</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="userData">userData</param>
        UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode, bool activeOnLoad, uint priority, object userData);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        UniTask UnloadSceneAsync(string location);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        UniTask UnloadSceneAsync(string location, object userData);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="handle">资源句柄</param>
        /// <param name="userData">用户自定义数据。</param>
        UniTask UnloadSceneAsync(SceneHandle handle, object userData);
    }
}

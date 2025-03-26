//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using AIOFramework.Event;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 场景组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("AIOFramework/Scene")]
    public sealed class SceneComponent : GameFrameworkComponent
    {
        private readonly string sceneAssetDirectory = "Assets/ArtAssets/Scene";
        private const int DefaultPriority = 0;
        private ISceneManager _sceneManager = null;
        private EventComponent _eventComponent = null;
        private SceneProxy _curSceneProxy = null;
        public SceneProxy CurSceneProxy => _curSceneProxy;


        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _sceneManager = GameFrameworkEntry.GetModule(typeof(SceneManager)) as ISceneManager;
            _sceneManager.LoadSceneSuccess += OnLoadSceneSuccess;
            _sceneManager.LoadSceneFailure += OnLoadSceneFailure;
            _sceneManager.UnloadSceneSuccess += OnUnloadSceneSuccess;
            _sceneManager.UnloadSceneFailure += OnUnloadSceneFailure;

        }

        private void Start()
        {
            _eventComponent = Entrance.Event;
            _sceneManager.SetResourceManager(Entrance.Resource);
        }


        /// <summary>
        /// 获取场景资源路径。
        /// </summary>
        /// <param name="name">场景资源名称。</param>
        /// <returns>场景资源路径。</returns>
        public string GetScenePath(string name){
            return $"{sceneAssetDirectory}/{name}.unity";
        }

        /// <summary>
        /// 获取场景名称。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景名称。</returns>
        public static string GetSceneName(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = location.LastIndexOf('/');
            if (sceneNamePosition + 1 >= location.Length)
            {
                Log.Error("Scene asset name '{0}' is invalid.", location);
                return null;
            }

            string sceneName = location.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string location)
        {
            return _sceneManager.SceneIsLoaded(location);
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的资源名称。</returns>
        public string[] GetLoadedScenes()
        {
            return _sceneManager.GetLoadedScenes();
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的资源名称。</param>
        public void GetLoadedScenes(List<string> results)
        {
            _sceneManager.GetLoadedScenes(results);
        }

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        public bool SceneIsLoading(string location)
        {
            return _sceneManager.SceneIsLoading(location);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <returns>正在加载场景的资源名称。</returns>
        public string[] GetLoadingScenes()
        {
            return _sceneManager.GetLoadingScenes();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <param name="results">正在加载场景的资源名称。</param>
        public void GetLoadingScenes(List<string> results)
        {
            _sceneManager.GetLoadingScenes(results);
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string location)
        {
            return _sceneManager.SceneIsUnloading(location);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的资源名称。</returns>
        public string[] GetUnloadingScenes()
        {
            return _sceneManager.GetUnloadingScenes();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称。</param>
        public void GetUnloadingScenes(List<string> results)
        {
            _sceneManager.GetUnloadingScenes(results);
        }

        /// <summary>
        /// 检查场景资源是否存在。
        /// </summary>
        /// <param name="location">要检查场景资源的名称。</param>
        /// <returns>场景资源是否存在。</returns>
        public bool HasScene(string location)
        {
            return _sceneManager.SceneIsLoaded(location);
        }


        public SceneProxy TryGetSceneProxy(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            var sceneHandle = _sceneManager.GetLoadedSceneHandle(location);
            if (sceneHandle == null) return null;

            var rootGameObjects = sceneHandle.SceneObject.GetRootGameObjects();
            var sceneProxy = rootGameObjects[0].GetComponent<SceneProxy>();

            return sceneProxy;
        }


        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        public async UniTask<SceneProxy> LoadSceneAsync(string location)
        {
            return await LoadSceneAsync(location, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        public async UniTask<SceneProxy> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            return await LoadSceneAsync(location, mode, 0, null);
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        /// <param name="activeOnLoad">true=加载完场景后直接激活.false=加载到90%时挂起</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="userData">userData</param>
        public async UniTask<SceneProxy> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode, uint priority, object userData)
        {
            var sceneHandle = await _sceneManager.LoadSceneAsync(location, mode, true, priority, userData);
            if (sceneHandle == null || !sceneHandle.IsValid)
            {
                Log.Error($"[SceneComponent] LoadSceneAsync failed, location: {location}");
                return null;
            }
            if (!sceneHandle.SceneObject.IsValid()) //如果都用await调用,理论上不会进入该分支
            {
                Log.Error($"[SceneComponent] SceneObject is invalid, location: {location}");
                return null;
            }
            var rootGameObjects = sceneHandle.SceneObject.GetRootGameObjects();
            var sceneProxy = rootGameObjects[0].GetComponent<SceneProxy>();
            if (sceneProxy == null)
            {
                Log.Error($"[SceneComponent] SceneProxy is null");
                return null;
            }
            sceneProxy.SceneHandle = sceneHandle;
            FocusScene(sceneProxy);

            return sceneProxy;
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="proxy">场景代理。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns></returns>
        public async UniTask UnloadSceneAsync(SceneProxy proxy, object userData = null)
        {
            await UnloadSceneAsync(proxy.SceneHandle, userData);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="userData">用户自定义数据。</param>
        public async UniTask UnloadSceneAsync(string location, object userData = null)
        {
            await _sceneManager.UnloadSceneAsync(location, userData);
        }

        public async UniTask UnloadSceneAsync(SceneHandle handle, object userData = null)
        {
            if (handle == null)
            {
                Log.Error("[SceneComponent] Scene handle is invalid.");
                return;
            }

            await _sceneManager.UnloadSceneAsync(handle, userData);
        }


        private void FocusScene(SceneProxy proxy)
        {
            SceneProxy oldProxy = null;
            if (_curSceneProxy != null && _curSceneProxy != proxy)
            {
                oldProxy = _curSceneProxy;
            }
            _curSceneProxy = proxy;
            _curSceneProxy.OnFocus();
            if (oldProxy != null)
            {
                oldProxy.OnLooseFocus();
            }

            OnActiveSceneChanged(this, ActiveSceneChangedEventArgs.Create(oldProxy, _curSceneProxy));
        }

        private void OnActiveSceneChanged(object sender, ActiveSceneChangedEventArgs e)
        {
            _eventComponent.Fire(this, e);
        }

        private void OnLoadSceneSuccess(object sender, LoadSceneSuccessEventArgs e)
        {
            _eventComponent.Fire(this, e);
        }

        private void OnLoadSceneFailure(object sender, LoadSceneFailureEventArgs e)
        {
            _eventComponent.Fire(this, e);
        }

        private void OnUnloadSceneSuccess(object sender, UnloadSceneSuccessEventArgs e)
        {
            _eventComponent.Fire(this, e);
        }

        private void OnUnloadSceneFailure(object sender, UnloadSceneFailureEventArgs e)
        {
            _eventComponent.Fire(this, e);
        }
    }
}
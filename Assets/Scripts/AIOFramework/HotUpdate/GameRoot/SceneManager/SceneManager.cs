
using System;
using System.Collections.Generic;
using System.Linq;
using AIOFramework.Event;
using AIOFramework.Resource;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 场景管理器。
    /// </summary>
    public sealed class SceneManager : GameFrameworkModule, ISceneManager
    {
        private readonly Dictionary<string, SceneHandle> _loadedScenes;
        private readonly Dictionary<string, SceneHandle> _loadingScenes;
        private readonly Dictionary<string, SceneHandle> _unloadingScenes;
        private IAssetManager _assetManager;
        private EventHandler<LoadSceneSuccessEventArgs> _loadSceneSuccessEventHandler;
        private EventHandler<LoadSceneFailureEventArgs> _loadSceneFailureEventHandler;
        private EventHandler<UnloadSceneSuccessEventArgs> _unloadSceneSuccessEventHandler;
        private EventHandler<UnloadSceneFailureEventArgs> _unloadSceneFailureEventHandler;

        /// <summary>
        /// 初始化场景管理器的新实例。
        /// </summary>
        public SceneManager()
        {
            _loadedScenes = new Dictionary<string, SceneHandle>();
            _loadingScenes = new Dictionary<string, SceneHandle>();
            _unloadingScenes = new Dictionary<string, SceneHandle>();
            _assetManager = null;
            _loadSceneSuccessEventHandler = null;
            _loadSceneFailureEventHandler = null;
            _unloadSceneSuccessEventHandler = null;
            _unloadSceneFailureEventHandler = null;
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// 加载场景成功事件。
        /// </summary>
        public event EventHandler<LoadSceneSuccessEventArgs> LoadSceneSuccess
        {
            add
            {
                _loadSceneSuccessEventHandler += value;
            }
            remove
            {
                _loadSceneSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 加载场景失败事件。
        /// </summary>
        public event EventHandler<LoadSceneFailureEventArgs> LoadSceneFailure
        {
            add
            {
                _loadSceneFailureEventHandler += value;
            }
            remove
            {
                _loadSceneFailureEventHandler -= value;
            }
        }

        /// <summary>
        /// 卸载场景成功事件。
        /// </summary>
        public event EventHandler<UnloadSceneSuccessEventArgs> UnloadSceneSuccess
        {
            add
            {
                _unloadSceneSuccessEventHandler += value;
            }
            remove
            {
                _unloadSceneSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 卸载场景失败事件。
        /// </summary>
        public event EventHandler<UnloadSceneFailureEventArgs> UnloadSceneFailure
        {
            add
            {
                _unloadSceneFailureEventHandler += value;
            }
            remove
            {
                _unloadSceneFailureEventHandler -= value;
            }
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理场景管理器。
        /// </summary>
        internal override void Shutdown()
        {
            foreach (var valuePair in _loadedScenes)
            {
                if (SceneIsUnloading(valuePair.Key))
                {
                    continue;
                }
                _ = UnloadSceneAsync(valuePair.Key);
            }

            _loadedScenes.Clear();
            _loadingScenes.Clear();
            _unloadingScenes.Clear();
        }

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="assetManager">资源管理器。</param>
        public void SetResourceManager(IAssetManager assetManager)
        {
            if (assetManager == null)
            {
                Log.Error("[SceneManager] Resource manager is invalid.");
                return;
            }

            this._assetManager = assetManager;
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="location">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return false;
            }

            return GetLoadedSceneHandle(location) != null;
        }

        public SceneHandle GetLoadedSceneHandle(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return null;
            }

            return _loadedScenes.TryGetValue(location, out var handle) ? handle : null;
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的路径</returns>
        public string[] GetLoadedScenes()
        {
            return _loadedScenes.Keys.ToArray();
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的路径。</param>
        public void GetLoadedScenes(List<string> results)
        {
            if (results == null)
            {
                Log.Error("[SceneManager] Results is invalid.");
                return;
            }

            results.Clear();
            results.AddRange(_loadedScenes.Keys);
        }

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <returns>场景是否正在加载。</returns>
        public bool SceneIsLoading(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return false;
            }

            return GetLoadingSceneHandle(location) != null;
        }

        public SceneHandle GetLoadingSceneHandle(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return null;
            }

            return _loadingScenes.TryGetValue(location, out var handle) ? handle : null;
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <returns>正在加载场景的资源名称。</returns>
        public string[] GetLoadingScenes()
        {
            return _loadingScenes.Keys.ToArray();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <param name="results">正在加载场景的路径。</param>
        public void GetLoadingScenes(List<string> results)
        {
            if (results == null)
            {
                Log.Error("[SceneManager] Results is invalid.");
                return;
            }

            results.Clear();
            results.AddRange(_loadingScenes.Keys);
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return false;
            }

            return GetUnloadingSceneHandle(location) != null;
        }

        public SceneHandle GetUnloadingSceneHandle(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return null;
            }

            return _unloadingScenes.TryGetValue(location, out var handle) ? handle : null;
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的路径。</returns>
        public string[] GetUnloadingScenes()
        {
            return _unloadingScenes.Keys.ToArray();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的路径。</param>
        public void GetUnloadingScenes(List<string> results)
        {
            if (results == null)
            {
                Log.Error("[SceneManager] Results is invalid.");
                return;
            }

            results.Clear();
            results.AddRange(_unloadingScenes.Keys);
        }


        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        public async UniTask<SceneHandle> LoadSceneAsync(string location)
        {
            var result = await LoadSceneAsync(location, UnityEngine.SceneManagement.LoadSceneMode.Single);
            return result;
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        public async UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            var result = await LoadSceneAsync(location, mode, true);
            return result;
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        /// <param name="activeOnLoad">true=加载完场景后直接激活.false=加载到90%时挂起</param>
        public async UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode, bool activeOnLoad)
        {
            var result = await LoadSceneAsync(location, mode, activeOnLoad, 0, null);
            return result;
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="mode">场景加载模式。</param>
        /// <param name="activeOnLoad">true=加载完场景后直接激活.false=加载到90%时挂起</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="userData">userData</param>
        public async UniTask<SceneHandle> LoadSceneAsync(string location, UnityEngine.SceneManagement.LoadSceneMode mode, bool activeOnLoad, uint priority, object userData)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene location is invalid.");
                return null;
            }

            if (_assetManager == null)
            {
                Log.Error("[SceneManager] You must set resource manager first.");
                return null;
            }

            if (SceneIsUnloading(location)) //如果都用await调用,理论上不会进入该分支
            {
                Log.Warning($"[SceneManager] Scene {location} is being unloaded. Load operation will be done after unload operation");
            }

            if (SceneIsLoading(location)) //如果都用await调用,理论上不会进入该分支
            {
                Log.Warning($"[SceneManager] Scene {location} is being loaded. Load operation will be done after load operation");
                return GetLoadingSceneHandle(location);
            }

            if (SceneIsLoaded(location))
            {
                Log.Info($"[SceneManager] Scene {location} is already loaded.");
                var loadedHandle = GetLoadedSceneHandle(location);
                if (activeOnLoad)
                {
                    loadedHandle.ActivateScene();
                }
                return loadedHandle;
            }

            var handle = CreateSceneHandle(location, mode, activeOnLoad, priority);
            _loadingScenes.Add(location, handle);

            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed)
            {
                LoadSceneSuccessCallback(location, handle, userData);
                return handle;
            }
            else
            {
                LoadSceneFailureCallback(location, handle.LastError, userData);
                return null;
            } 
        }

        private SceneHandle CreateSceneHandle(string location, UnityEngine.SceneManagement.LoadSceneMode mode, bool activeOnLoad, uint priority)
        {
            return _assetManager.ResourcePackage.LoadSceneAsync(location, mode, UnityEngine.SceneManagement.LocalPhysicsMode.None, activeOnLoad == false, priority);
        }


        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        public async UniTask UnloadSceneAsync(string location)
        {
            await UnloadSceneAsync(location, null);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="location">场景资源路径。</param>
        /// <param name="userData">用户自定义数据。</param>
        public async UniTask UnloadSceneAsync(string location, object userData)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("[SceneManager] Scene asset name is invalid.");
                return;
            }

            if (_assetManager == null)
            {
                Log.Error("[SceneManager] You must set resource manager first.");
                return;
            }

            if (SceneIsUnloading(location))
            {
                Log.Info($"[SceneManager] Scene asset '{location}' is being unloaded.");
                return;
            }

            if (SceneIsLoading(location))
            {
                Log.Warning($"[SceneManager] Scene asset '{location}' is being loaded.");
                var handle = GetLoadingSceneHandle(location);
                await UnloadSceneAsync(handle, userData);
                return;
            }

            if (!SceneIsLoaded(location))
            {
                Log.Warning($"[SceneManager] Scene asset '{location}' is not loaded yet.");
                return;
            }

            var sceneHandle = GetLoadedSceneHandle(location);
            await UnloadSceneAsync(sceneHandle, userData);
        }


        public async UniTask UnloadSceneAsync(SceneHandle handle, object userData)
        {
            if (handle == null)
            {
                Log.Error("[SceneManager] Scene handle is invalid.");
                return;
            }

            var location = handle.GetAssetInfo().AssetPath;
            var unloadOperation = handle.UnloadAsync();
            await unloadOperation.ToUniTask();

            if (unloadOperation.Status == EOperationStatus.Succeed)
            {
                _loadedScenes.Remove(location);
                await _assetManager.UnloadUnusedAssetsAsync();
                UnloadSceneSuccessCallback(location, userData);
            }
            else
            {
                UnloadSceneFailureCallback(location, userData);
            }
        }

        private void LoadSceneSuccessCallback(string location, SceneHandle handle, object userData)
        {
            _loadingScenes.Remove(location);
            _loadedScenes.Add(location, handle);
            Log.Info($"[SceneManager] LoadSceneSuccess {location}");
            if (_loadSceneSuccessEventHandler != null)
            {
                LoadSceneSuccessEventArgs loadSceneSuccessEventArgs = LoadSceneSuccessEventArgs.Create(location, handle, userData);
                _loadSceneSuccessEventHandler(this, loadSceneSuccessEventArgs);
                ReferencePool.Release(loadSceneSuccessEventArgs);
            }
        }

        private void LoadSceneFailureCallback(string location, string errorMessage, object userData)
        {
            _loadingScenes.Remove(location);
            string appendErrorMessage = $"[SceneManager] Load scene failure, scene asset name '{location}', error message '{errorMessage}'.";
            Log.Error(appendErrorMessage);
            if (_loadSceneFailureEventHandler != null)
            {
                LoadSceneFailureEventArgs loadSceneFailureEventArgs = LoadSceneFailureEventArgs.Create(location, appendErrorMessage, userData);
                _loadSceneFailureEventHandler(this, loadSceneFailureEventArgs);
                ReferencePool.Release(loadSceneFailureEventArgs);
                return;
            }
        }


        private void UnloadSceneSuccessCallback(string location, object userData)
        {
            _unloadingScenes.Remove(location);
            _loadedScenes.Remove(location);
            Log.Info($"[SceneManager] UnloadSceneSuccess {location}");
            if (_unloadSceneSuccessEventHandler != null)
            {
                UnloadSceneSuccessEventArgs unloadSceneSuccessEventArgs = UnloadSceneSuccessEventArgs.Create(location, userData);
                _unloadSceneSuccessEventHandler(this, unloadSceneSuccessEventArgs);
                ReferencePool.Release(unloadSceneSuccessEventArgs);
            }
        }

        private void UnloadSceneFailureCallback(string location, object userData)
        {
            _unloadingScenes.Remove(location);
            Log.Error($"[SceneManager] Unload scene failure, scene asset name '{location}'.");
            if (_unloadSceneFailureEventHandler != null)
            {
                UnloadSceneFailureEventArgs unloadSceneFailureEventArgs = UnloadSceneFailureEventArgs.Create(location, userData);
                _unloadSceneFailureEventHandler(this, unloadSceneFailureEventArgs);
                ReferencePool.Release(unloadSceneFailureEventArgs);
                return;
            }
        }
    }
}

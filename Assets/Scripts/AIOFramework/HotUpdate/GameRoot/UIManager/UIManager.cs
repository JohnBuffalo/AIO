using System;
using System.Collections.Generic;
using AIOFramework.ObjectPool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using AIOFramework.Runtime;
using AIOFramework.Resource;
using AIOFramework.Event;
using YooAsset;

namespace AIOFramework.UI
{
    public class UIManager : GameFrameworkModule, IUIManager
    {
        private readonly Dictionary<UIGroupEnum, UIGroup> _uiGroups;
        private readonly Dictionary<int, string> _uiBeingLoaded;
        private readonly HashSet<int> _uiToReleaseOnLoad;
        private readonly Queue<IUIForm> _recycleQueue;
        private IObjectPoolManager _objectPoolManager;
        private IAssetManager _assetManager;
        private int _serial;
        private IObjectPool<UIInstanceObject> _instancePool;
        private EventHandler<OpenUISuccessEventArgs> _openUISuccessEventHandler;
        private EventHandler<OpenUIFailureEventArgs> _openUIFailureEventHandler;
        private EventHandler<CloseUICompleteEventArgs> _closeUICompleteEventHandler;

        public UIManager()
        {
            _uiGroups = new Dictionary<UIGroupEnum, UIGroup>();
            _uiBeingLoaded = new Dictionary<int, string>();
            _recycleQueue = new Queue<IUIForm>();
            _uiToReleaseOnLoad = new HashSet<int>();
            _objectPoolManager = null;
            _assetManager = null;
            _serial = 0;
            _instancePool = null;
            _openUISuccessEventHandler = null;
            _openUIFailureEventHandler = null;
            _closeUICompleteEventHandler = null;
        }

        public int UIGroupCount
        {
            get { return _uiGroups.Count; }
        }

        public float InstanceAutoReleaseInterval
        {
            get { return _instancePool.AutoReleaseInterval; }
            set { _instancePool.AutoReleaseInterval = value; }
        }

        public int InstanceCapacity
        {
            get { return _instancePool.Capacity; }
            set { _instancePool.Capacity = value; }
        }

        public float InstanceExpireTime
        {
            get { return _instancePool.ExpireTime; }
            set { _instancePool.ExpireTime = value; }
        }

        public int InstancePriority
        {
            get { return _instancePool.Priority; }
            set { _instancePool.Priority = value; }
        }

        public event EventHandler<OpenUISuccessEventArgs> OpenUIFormSuccess
        {
            add { _openUISuccessEventHandler += value; }
            remove { _openUISuccessEventHandler -= value; }
        }

        public event EventHandler<OpenUIFailureEventArgs> OpenUIFormFailure
        {
            add { _openUIFailureEventHandler += value; }
            remove { _openUIFailureEventHandler -= value; }
        }

        public event EventHandler<CloseUICompleteEventArgs> CloseUIFormComplete
        {
            add { _closeUICompleteEventHandler += value; }
            remove { _closeUICompleteEventHandler -= value; }
        }


        /// <summary>
        /// 界面管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (_recycleQueue.Count > 0)
            {
                IUIForm uiForm = _recycleQueue.Dequeue();
                uiForm.OnRecycle();

                ((UIViewBase)uiForm).SetActive(false);
                UIGroup poolGroup = (UIGroup)GetUIGroup(UIGroupEnum.Pool);
                poolGroup.AddUI((UIViewBase)uiForm);
                ((UIViewBase)uiForm).UIGroup = poolGroup;
                ((GameObject)uiForm.Handle).transform.SetParent(((MonoBehaviour)poolGroup.Helper).transform);
                poolGroup.Refresh();

                _instancePool.Unspawn(uiForm.Handle);
            }

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                uiGroup.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        internal override void Shutdown()
        {
            CloseAllLoadedUI();
            _uiGroups.Clear();
            _uiBeingLoaded.Clear();
            _uiToReleaseOnLoad.Clear();
            _recycleQueue.Clear();
        }

        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            if (objectPoolManager == null)
            {
                throw new GameFrameworkException("Object pool manager is invalid.");
            }

            this._objectPoolManager = objectPoolManager;
            _instancePool = this._objectPoolManager.CreateSingleSpawnObjectPool<UIInstanceObject>("UI Instance Pool");
        }

        public void SetAssetManager(IAssetManager assetManager)
        {
            if (assetManager == null)
            {
                throw new GameFrameworkException("Resource manager is invalid.");
            }

            this._assetManager = assetManager;
        }

        public bool HasUIGroup(UIGroupEnum uiGroupName)
        {
            return _uiGroups.ContainsKey(uiGroupName);
        }

        public IUIGroup GetUIGroup(UIGroupEnum uiGroupName)
        {
            UIGroup uiGroup = null;
            if (_uiGroups.TryGetValue(uiGroupName, out uiGroup))
            {
                return uiGroup;
            }

            return null;
        }

        public IUIGroup[] GetAllUIGroups()
        {
            int index = 0;
            IUIGroup[] results = new IUIGroup[_uiGroups.Count];
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                results[index++] = uiGroup.Value;
            }

            return results;
        }

        public void GetAllUIGroups(List<IUIGroup> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                results.Add(uiGroup.Value);
            }
        }

        public bool AddUIGroup(UIGroupEnum uiGroupName, int uiGroupDepth, IUIGroupHelper uiGroupHelper)
        {
            if (uiGroupHelper == null)
            {
                throw new GameFrameworkException("UI group helper is invalid.");
            }

            if (HasUIGroup(uiGroupName))
            {
                return false;
            }

            _uiGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, uiGroupHelper));

            return true;
        }

        public bool HasUI(int serialId)
        {
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                if (uiGroup.Value.HasUI(serialId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasUI(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                if (uiGroup.Value.HasUI(uiFormAssetName))
                {
                    return true;
                }
            }

            return false;
        }

        public T GetUI<T>(int serialId) where T : IUIForm
        {
            var ui = GetUI(serialId);
            return (T)ui;
        }

        public T GetUI<T>(string assetName) where T : IUIForm
        {
            var ui = GetUI(assetName);
            return (T)ui;
        }

        public IUIForm GetUI(int serialId)
        {
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                IUIForm uiForm = uiGroup.Value.GetUI(serialId);
                if (uiForm != null)
                {
                    return uiForm;
                }
            }

            return null;
        }

        public IUIForm GetUI(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                IUIForm uiForm = uiGroup.Value.GetUI(uiFormAssetName);
                if (uiForm != null)
                {
                    return uiForm;
                }
            }

            return null;
        }

        public IUIForm[] GetAllUI(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            List<IUIForm> results = new List<IUIForm>();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                results.AddRange(uiGroup.Value.GetUIArray(uiAssetName));
            }

            return results.ToArray();
        }

        public void GetAllUI(string uiFormAssetName, List<IUIForm> results)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                uiGroup.Value.InternalGetUIList(uiFormAssetName, results);
            }
        }

        public IUIForm[] GetAllLoadedUI()
        {
            List<IUIForm> results = new List<IUIForm>();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                results.AddRange(uiGroup.Value.GetAllUI());
            }

            return results.ToArray();
        }

        public void GetAllLoadedUI(List<IUIForm> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in _uiGroups)
            {
                uiGroup.Value.InternalGetAllUIList(results);
            }
        }

        public int[] GetAllLoadingUISerialIds()
        {
            int index = 0;
            int[] results = new int[_uiBeingLoaded.Count];
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in _uiBeingLoaded)
            {
                results[index++] = uiFormBeingLoaded.Key;
            }

            return results;
        }

        public void GetAllLoadingUISerialIds(List<int> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in _uiBeingLoaded)
            {
                results.Add(uiFormBeingLoaded.Key);
            }
        }

        public bool IsLoadingUI(int serialId)
        {
            return _uiBeingLoaded.ContainsKey(serialId);
        }

        public bool IsLoadingUI(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            return _uiBeingLoaded.ContainsValue(uiFormAssetName);
        }

        public bool IsValidUI(IUIForm uiForm)
        {
            if (uiForm == null)
            {
                return false;
            }

            return HasUI(uiForm.SerialId);
        }

        public async UniTask<int> OpenUI<TViewModel>(UICtorInfo ctorInfo) where TViewModel : UIViewModelBase, new()
        {
            var viewModel = ReferencePool.Acquire<TViewModel>();
            var pageUid = await OpenUI(ctorInfo, viewModel);
            return pageUid;
        }


        public async UniTask<int> OpenUI(UICtorInfo ctorInfo, UIViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new GameFrameworkException("ViewModel is invalid.");
            }

            if (ctorInfo == null)
            {
                throw new GameFrameworkException("CtorInfo is invalid.");
            }

            if (_assetManager == null)
            {
                throw new GameFrameworkException("You must set asset manager first.");
            }

            if (string.IsNullOrEmpty(ctorInfo.Location))
            {
                throw new GameFrameworkException("UI form asset location is invalid.");
            }

            int serialId = ++_serial;
            string uiName = GetUIName(ctorInfo.Location);
            UIGroup uiGroup = (UIGroup)GetUIGroup(ctorInfo.Group);
            Log.Info($"[UIManager] OpenUI {uiName}");
            if (!ctorInfo.Multiple) //只允许有一个同类UI
            {
                var existUI = (UIViewBase)uiGroup.GetUI(uiName);
                if (existUI != null)
                {
                    Log.Info($"[UIManager] existUI {uiName} at {existUI.SerialId}");
                    _serial--;
                    RefocusUI(existUI, ctorInfo);
                    ReferencePool.Release(viewModel);
                    return existUI.SerialId;
                }
            }

            UIInstanceObject uiInstanceObject = _instancePool.Spawn(uiName);
            if (uiInstanceObject == null)
            {
                _uiBeingLoaded.Add(serialId, ctorInfo.Location);
                var loadResult = await _assetManager.LoadAssetAsync<GameObject>(ctorInfo.Location);
                uiInstanceObject = LoadAssetSuccessCallback(loadResult.Item2, loadResult.Item1, serialId);
            }

            if (uiInstanceObject != null)
            {
                InternalOpenUI(serialId, uiName, ctorInfo, (GameObject)uiInstanceObject.Target, viewModel);
            }

            return serialId;
        }

        private UIInstanceObject LoadAssetSuccessCallback(AssetHandle loadHandle, GameObject uiAsset, int serialId)
        {
            if (_uiToReleaseOnLoad.Contains(serialId))
            {
                _uiToReleaseOnLoad.Remove(serialId);
                _assetManager.UnloadAsset(loadHandle);
                return null;
            }

            var location = loadHandle.Provider.MainAssetInfo.AssetPath;
            _uiBeingLoaded.Remove(serialId);
            GameObject uiInstance = GameObject.Instantiate(uiAsset);
            UIInstanceObject uiInstanceObject =
                UIInstanceObject.Create(GetUIName(location), location, uiInstance, loadHandle);
            _instancePool.Register(uiInstanceObject, true);

            return uiInstanceObject;
        }


        private void InternalOpenUI(int serialId, string uiName, UICtorInfo ctorInfo, GameObject uiInstance,
            UIViewModelBase viewModel)
        {
            UIGroup uiGroup = (UIGroup)GetUIGroup(ctorInfo.Group);
            Transform uiRoot = ((MonoBehaviour)uiGroup.Helper).transform;
            try
            {
                UIViewBase uiView = uiInstance.GetComponent<UIViewBase>();

                if (uiView == null)
                {
                    throw new GameFrameworkException("Can not create UI form in UI form helper.");
                }

                uiInstance.transform.SetParent(uiRoot);
                uiInstance.GetComponent<RectTransform>().localPosition = Vector3.zero;
                uiInstance.GetComponent<RectTransform>().localScale = Vector3.one;
                uiInstance.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                uiView.OnInit(serialId, uiName, uiGroup, viewModel, ctorInfo);
                uiGroup.AddUI(uiView);
                uiView.OnOpen(null);
                uiGroup.Refresh();

                if (_openUISuccessEventHandler != null)
                {
                    _openUISuccessEventHandler(this, OpenUISuccessEventArgs.Create(uiView, null));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                if (_openUIFailureEventHandler != null)
                {
                    _openUIFailureEventHandler(this, OpenUIFailureEventArgs.Create(serialId, uiName, uiGroup.Name, e.ToString(), null));
                }
            }
        }

        private string GetUIName(string location)
        {
            return Utility.Path.GetFileNameWithoutExtension(location);
        }

        public void CloseUI(int serialId)
        {
            CloseUI(serialId, null);
        }

        public void CloseUI(int serialId, object userData)
        {
            if (IsLoadingUI(serialId))
            {
                _uiToReleaseOnLoad.Add(serialId);
                _uiBeingLoaded.Remove(serialId);
                return;
            }

            IUIForm uiView = GetUI(serialId);
            if (uiView == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not find UI form '{0}'.", serialId));
            }

            CloseUI(uiView, userData);
        }

        public void CloseUI(IUIForm uiForm)
        {
            CloseUI(uiForm, null);
        }

        public void CloseUI(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
            {
                throw new GameFrameworkException("UI is invalid.");
            }

            UIGroup uiGroup = (UIGroup)uiForm.UIGroup;
            if (uiGroup == null)
            {
                throw new GameFrameworkException("UI group is invalid.");
            }

            uiGroup.RemoveUI((UIViewBase)uiForm);
            uiForm.OnClose(userData);
            uiGroup.Refresh();

            if (_closeUICompleteEventHandler != null)
            {
                _closeUICompleteEventHandler(this, CloseUICompleteEventArgs.Create(uiForm.SerialId, uiForm.UIAssetName, uiGroup, userData));
            }

            _recycleQueue.Enqueue(uiForm);
        }

        public void CloseAllLoadedUI()
        {
            CloseAllLoadedUI(null);
        }

        public void CloseAllLoadedUI(object userData)
        {
            IUIForm[] uiForms = GetAllLoadedUI();
            foreach (IUIForm uiForm in uiForms)
            {
                if (!HasUI(uiForm.SerialId))
                {
                    continue;
                }

                CloseUI(uiForm, userData);
            }
        }

        public void CloseAllLoadingUI()
        {
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in _uiBeingLoaded)
            {
                _uiToReleaseOnLoad.Add(uiFormBeingLoaded.Key);
            }

            _uiBeingLoaded.Clear();
        }

        public void RefocusUI(IUIForm uiForm)
        {
            RefocusUI(uiForm, null);
        }

        public void RefocusUI(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
            {
                throw new GameFrameworkException("UI form is invalid.");
            }

            UIGroup uiGroup = (UIGroup)uiForm.UIGroup;
            if (uiGroup == null)
            {
                throw new GameFrameworkException("UI group is invalid.");
            }

            uiGroup.RefocusUI((UIViewBase)uiForm);
            uiGroup.Refresh();
            uiForm.OnRefocus(userData);
        }

        public void SetUIInstanceLocked(object uiInstance, bool locked)
        {
            if (uiInstance == null)
            {
                throw new GameFrameworkException("UI instance is invalid.");
            }

            _instancePool.SetLocked(uiInstance, locked);
        }

        public void SetUIInstancePriority(object uiInstance, int priority)
        {
            if (uiInstance == null)
            {
                throw new GameFrameworkException("UI instance is invalid.");
            }

            _instancePool.SetPriority(uiInstance, priority);
        }
    }
}
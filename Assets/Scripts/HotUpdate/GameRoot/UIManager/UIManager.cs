using System;
using System.Collections.Generic;
using System.Reflection;
using AIOFramework.ObjectPool;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;
using UnityEngine;

namespace AIOFramework.Runtime
{
    public class UIManager : GameFrameworkModule, IUIManager
    {
        private readonly Dictionary<UIGroupEnum, UIGroup> m_UIGroups;
        private readonly Dictionary<int, string> m_UIBeingLoaded;
        private readonly HashSet<int> m_UIToReleaseOnLoad;
        private readonly Queue<IUIForm> m_RecycleQueue;
        private IObjectPoolManager m_ObjectPoolManager;
        private IAssetManager m_AssetManager;
        private int m_Serial;
        private IObjectPool<UIInstanceObject> m_InstancePool;
        private EventHandler<OpenUISuccessEventArgs> m_OpenUISuccessEventHandler;
        private EventHandler<OpenUIFailureEventArgs> m_OpenUIFailureEventHandler;
        private EventHandler<CloseUICompleteEventArgs> m_CloseUICompleteEventHandler;

        public UIManager()
        {
            m_UIGroups = new Dictionary<UIGroupEnum, UIGroup>();
            m_UIBeingLoaded = new Dictionary<int, string>();
            m_RecycleQueue = new Queue<IUIForm>();
            m_UIToReleaseOnLoad = new HashSet<int>();
            m_ObjectPoolManager = null;
            m_AssetManager = null;
            m_Serial = 0;
            m_InstancePool = null;
            m_OpenUISuccessEventHandler = null;
            m_OpenUIFailureEventHandler = null;
            m_CloseUICompleteEventHandler = null;
        }

        public int UIGroupCount
        {
            get { return m_UIGroups.Count; }
        }

        public float InstanceAutoReleaseInterval
        {
            get { return m_InstancePool.AutoReleaseInterval; }
            set { m_InstancePool.AutoReleaseInterval = value; }
        }

        public int InstanceCapacity
        {
            get { return m_InstancePool.Capacity; }
            set { m_InstancePool.Capacity = value; }
        }

        public float InstanceExpireTime
        {
            get { return m_InstancePool.ExpireTime; }
            set { m_InstancePool.ExpireTime = value; }
        }

        public int InstancePriority
        {
            get { return m_InstancePool.Priority; }
            set { m_InstancePool.Priority = value; }
        }

        public event EventHandler<OpenUISuccessEventArgs> OpenUIFormSuccess
        {
            add { m_OpenUISuccessEventHandler += value; }
            remove { m_OpenUISuccessEventHandler -= value; }
        }

        public event EventHandler<OpenUIFailureEventArgs> OpenUIFormFailure
        {
            add { m_OpenUIFailureEventHandler += value; }
            remove { m_OpenUIFailureEventHandler -= value; }
        }

        public event EventHandler<CloseUICompleteEventArgs> CloseUIFormComplete
        {
            add { m_CloseUICompleteEventHandler += value; }
            remove { m_CloseUICompleteEventHandler -= value; }
        }


        /// <summary>
        /// 界面管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (m_RecycleQueue.Count > 0)
            {
                IUIForm uiForm = m_RecycleQueue.Dequeue();
                uiForm.OnRecycle();
                
                ((UIViewBase)uiForm).SetActive(false);
                UIGroup poolGroup = (UIGroup)GetUIGroup(UIGroupEnum.Pool);
                poolGroup.AddUI((UIViewBase)uiForm);
                ((UIViewBase)uiForm).UIGroup = poolGroup;
                ((GameObject)uiForm.Handle).transform.SetParent(((MonoBehaviour)poolGroup.Helper).transform);
                poolGroup.Refresh();
                
                m_InstancePool.Unspawn(uiForm.Handle);
            }

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        internal override void Shutdown()
        {
            CloseAllLoadedUI();
            m_UIGroups.Clear();
            m_UIBeingLoaded.Clear();
            m_UIToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }

        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            if (objectPoolManager == null)
            {
                throw new GameFrameworkException("Object pool manager is invalid.");
            }

            m_ObjectPoolManager = objectPoolManager;
            m_InstancePool = m_ObjectPoolManager.CreateSingleSpawnObjectPool<UIInstanceObject>("UI Instance Pool");
        }

        public void SetAssetManager(IAssetManager assetManager)
        {
            if (assetManager == null)
            {
                throw new GameFrameworkException("Resource manager is invalid.");
            }

            m_AssetManager = assetManager;
        }

        public bool HasUIGroup(UIGroupEnum uiGroupName)
        {
            return m_UIGroups.ContainsKey(uiGroupName);
        }

        public IUIGroup GetUIGroup(UIGroupEnum uiGroupName)
        {
            UIGroup uiGroup = null;
            if (m_UIGroups.TryGetValue(uiGroupName, out uiGroup))
            {
                return uiGroup;
            }

            return null;
        }

        public IUIGroup[] GetAllUIGroups()
        {
            int index = 0;
            IUIGroup[] results = new IUIGroup[m_UIGroups.Count];
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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

            m_UIGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, uiGroupHelper));

            return true;
        }

        public bool HasUI(int serialId)
        {
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUI(uiFormAssetName))
                {
                    return true;
                }
            }

            return false;
        }

        public T GetUI<T>(int serialId) where T :  IUIForm
        {
            var ui = GetUI(serialId);
            return (T)ui;
        }
        
        public T GetUI<T>(string assetName) where T :  IUIForm
        {
            var ui = GetUI(assetName);
            return (T)ui;
        }
        
        public IUIForm GetUI(int serialId)
        {
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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

            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.InternalGetUIList(uiFormAssetName, results);
            }
        }

        public IUIForm[] GetAllLoadedUI()
        {
            List<IUIForm> results = new List<IUIForm>();
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
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
            foreach (KeyValuePair<UIGroupEnum, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.InternalGetAllUIList(results);
            }
        }

        public int[] GetAllLoadingUISerialIds()
        {
            int index = 0;
            int[] results = new int[m_UIBeingLoaded.Count];
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIBeingLoaded)
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
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIBeingLoaded)
            {
                results.Add(uiFormBeingLoaded.Key);
            }
        }

        public bool IsLoadingUI(int serialId)
        {
            return m_UIBeingLoaded.ContainsKey(serialId);
        }

        public bool IsLoadingUI(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new GameFrameworkException("UI form asset name is invalid.");
            }

            return m_UIBeingLoaded.ContainsValue(uiFormAssetName);
        }

        public bool IsValidUI(IUIForm uiForm)
        {
            if (uiForm == null)
            {
                return false;
            }

            return HasUI(uiForm.SerialId);
        }

        /// <summary>
        /// 异步打开UI界面
        /// </summary>
        /// <param name="userData">界面初始化参数</param>
        /// <typeparam name="TUICtorInfo">页面配置信息</typeparam>
        /// <typeparam name="TViewModel">页面控制模块</typeparam>
        /// <returns></returns>
        public async UniTask<int> OpenUI<TUICtorInfo, TViewModel>(object userData = null)
            where TUICtorInfo : UICtorInfo, new() where TViewModel : UIViewModelBase, new()
        {
            var pageCtorInfo = ReferencePool.Acquire<TUICtorInfo>();
            var viewModel = ReferencePool.Acquire<TViewModel>();
            var pageUid = await OpenUI(pageCtorInfo, viewModel, userData);
            return pageUid;
        }

        public async UniTask<int> OpenUI(UICtorInfo ctorInfo, UIViewModelBase viewModel,
            object userData)
        {
            if (viewModel == null)
            {
                throw new GameFrameworkException("ViewModel is invalid.");
            }

            if (ctorInfo == null)
            {
                throw new GameFrameworkException("CtorInfo is invalid.");
            }

            if (m_AssetManager == null)
            {
                throw new GameFrameworkException("You must set asset manager first.");
            }

            if (string.IsNullOrEmpty(ctorInfo.Location))
            {
                throw new GameFrameworkException("UI form asset location is invalid.");
            }

            int serialId = ++m_Serial;
            string uiName = GetUIName(ctorInfo.Location);

            if (!ctorInfo.Multiple) //只允许有一个同类UI
            {
                var exsitUI = (UIViewBase)GetUI(uiName);
                if (exsitUI != null)
                {
                    m_Serial--;
                    UIGroup uiGroup = (UIGroup)exsitUI.UIGroup;
                    uiGroup.RemoveUI(exsitUI);
                    ReferencePool.Release(exsitUI.ViewModel);
                    exsitUI.ViewModel = null;
                    InternalOpenUI(exsitUI.SerialId, uiName, ctorInfo, (GameObject)exsitUI.Handle, viewModel, userData);
                    return exsitUI.SerialId;
                }
            }

            UIInstanceObject uiInstanceObject = m_InstancePool.Spawn(uiName);
            if (uiInstanceObject == null)
            {
                m_UIBeingLoaded.Add(serialId, ctorInfo.Location);
                GameObject pagePrefab = await m_AssetManager.LoadAssetAsync<GameObject>(ctorInfo.Location);
                uiInstanceObject = LoadAssetSuccessCallback(ctorInfo, pagePrefab, serialId);
            }
            
            if (uiInstanceObject != null)
            {
                InternalOpenUI(serialId, uiName, ctorInfo, (GameObject)uiInstanceObject.Target, viewModel, userData);
            }

            ReferencePool.Release(ctorInfo);
            return serialId;
        }

        private UIInstanceObject LoadAssetSuccessCallback(UICtorInfo ctorInfo, GameObject uiAsset, int serialId)
        {
            if (m_UIToReleaseOnLoad.Contains(serialId))
            {
                m_UIToReleaseOnLoad.Remove(serialId);
                m_AssetManager.UnloadAsset(ctorInfo.Location);
                return null;
            }

            m_UIBeingLoaded.Remove(serialId);
            GameObject uiInstance = GameObject.Instantiate(uiAsset);
            UIInstanceObject uiInstanceObject =
                UIInstanceObject.Create(GetUIName(ctorInfo.Location), ctorInfo.Location, uiInstance, m_AssetManager);
            m_InstancePool.Register(uiInstanceObject, true);

            return uiInstanceObject;
        }
        

        private void InternalOpenUI(int serialId, string uiName, UICtorInfo ctorInfo, GameObject uiInstance,
            UIViewModelBase viewModel, object userData)
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
                uiInstance.transform.localPosition = Vector3.zero;
                uiInstance.transform.localScale = Vector3.one;
                uiView.OnInit(serialId, uiName, uiGroup, viewModel, ctorInfo);
                uiGroup.AddUI(uiView);
                uiView.OnOpen(userData);
                uiGroup.Refresh();

                if (m_OpenUISuccessEventHandler != null)
                {
                    OpenUISuccessEventArgs args = OpenUISuccessEventArgs.Create(uiView, userData);
                    m_OpenUISuccessEventHandler(this, args);
                    ReferencePool.Release(args);
                }
            }
            catch (Exception e)
            {
                if (m_OpenUIFailureEventHandler != null)
                {
                    OpenUIFailureEventArgs args =
                        OpenUIFailureEventArgs.Create(serialId, uiName, uiGroup.Name, e.ToString(), userData);
                    m_OpenUIFailureEventHandler(this, args);
                    ReferencePool.Release(args);
                    return;
                }

                throw;
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
                m_UIToReleaseOnLoad.Add(serialId);
                m_UIBeingLoaded.Remove(serialId);
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
            
            if (m_CloseUICompleteEventHandler != null)
            {
                CloseUICompleteEventArgs args =
                    CloseUICompleteEventArgs.Create(uiForm.SerialId, uiForm.UIAssetName, uiGroup, userData);
                m_CloseUICompleteEventHandler(this, args);
                ReferencePool.Release(args);
            }

            m_RecycleQueue.Enqueue(uiForm);
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
            foreach (KeyValuePair<int, string> uiFormBeingLoaded in m_UIBeingLoaded)
            {
                m_UIToReleaseOnLoad.Add(uiFormBeingLoaded.Key);
            }

            m_UIBeingLoaded.Clear();
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

            m_InstancePool.SetLocked(uiInstance, locked);
        }

        public void SetUIInstancePriority(object uiInstance, int priority)
        {
            if (uiInstance == null)
            {
                throw new GameFrameworkException("UI instance is invalid.");
            }

            m_InstancePool.SetPriority(uiInstance, priority);
        }
    }
}
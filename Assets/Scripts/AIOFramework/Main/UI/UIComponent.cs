using System;
using System.Collections.Generic;
using AIOFramework.Event;
using AIOFramework.ObjectPool;
using AIOFramework.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace AIOFramework.UI
{
    /// <summary>
    /// 流程组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("AIOFramework/UI")]
    public partial class UIComponent : GameFrameworkComponent
    {
        IUIManager _uiManager;
        private Transform _instanceRoot = null;

        [SerializeField] private string _uiGroupHelperTypeName = "AIOFramework.UI.DefaultUIGroupHelper";

        // [SerializeField] private UIGroupDisplay[] m_UIGroups = null;
        private readonly List<IUIForm> _internalUIFormResults = new List<IUIForm>();
        [SerializeField] private float _instanceAutoReleaseInterval = 60f;
        [SerializeField] private int _instanceCapacity = 16;
        [SerializeField] private float _instanceExpireTime = 60f;

        public IUIManager UIManager
        {
            set { _uiManager = value; }
            get { return _uiManager; }
        }

        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        public int UIGroupCount
        {
            get { return _uiManager.UIGroupCount; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get { return _uiManager.InstanceAutoReleaseInterval; }
            set { _uiManager.InstanceAutoReleaseInterval = _instanceAutoReleaseInterval = value; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        public int InstanceCapacity
        {
            get { return _uiManager.InstanceCapacity; }
            set { _uiManager.InstanceCapacity = _instanceCapacity = value; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        public float InstanceExpireTime
        {
            get { return _uiManager.InstanceExpireTime; }
            set { _uiManager.InstanceExpireTime = _instanceExpireTime = value; }
        }


        private void OnOpenUIFormSuccess(object sender, OpenUISuccessEventArgs args)
        {
            Entrance.Event.Fire(this, args);
        }

        private void OnOpenUIFormFailure(object sender, OpenUIFailureEventArgs args)
        {
            Entrance.Event.Fire(this, args);
        }

        private void OnCloseUIFormComplete(object sender, CloseUICompleteEventArgs args)
        {
            Entrance.Event.Fire(this, args);
        }

        public void Init(Type uiManagerType, Transform insRoot)
        {
            _uiManager = GameFrameworkEntry.GetModule(uiManagerType) as IUIManager;
            if (_uiManager == null)
            {
                Log.Fatal("UI manager is null.");
                return;
            }

            _uiManager.SetAssetManager(Entrance.Resource);
            _uiManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
            _uiManager.InstanceAutoReleaseInterval = _instanceAutoReleaseInterval;
            _uiManager.InstanceCapacity = _instanceCapacity;
            _uiManager.InstanceExpireTime = _instanceExpireTime;
            _uiManager.OpenUIFormSuccess += OnOpenUIFormSuccess;
            _uiManager.OpenUIFormFailure += OnOpenUIFormFailure;
            _uiManager.CloseUIFormComplete += OnCloseUIFormComplete;

            if (_instanceRoot == null)
            {
                _instanceRoot = insRoot;
                _instanceRoot.SetParent(gameObject.transform);
                _instanceRoot.localScale = Vector3.one;
            }

            _instanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");

            foreach (UIGroupEnum uiGroup in Enum.GetValues(typeof(UIGroupEnum)))
            {
                if (UIGroupSorting.TryGetValue(uiGroup, out int depth))
                {
                    if (!AddUIGroup(uiGroup, depth))
                    {
                        Log.Warning("Add UI group '{0}' failure.", uiGroup);
                        continue;
                    }
                }
            }

            GetUIGroup(UIGroupEnum.Pool).Pause = true;
        }

        /// <summary>
        /// 是否存在界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>是否存在界面组。</returns>
        public bool HasUIGroup(UIGroupEnum uiGroupName)
        {
            return _uiManager.HasUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>要获取的界面组。</returns>
        public IUIGroup GetUIGroup(UIGroupEnum uiGroupName)
        {
            return _uiManager.GetUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取所有界面组。
        /// </summary>
        /// <returns>所有界面组。</returns>
        public IUIGroup[] GetAllUIGroups()
        {
            return _uiManager.GetAllUIGroups();
        }

        /// <summary>
        /// 获取所有界面组。
        /// </summary>
        /// <param name="results">所有界面组。</param>
        public void GetAllUIGroups(List<IUIGroup> results)
        {
            _uiManager.GetAllUIGroups(results);
        }

        /// <summary>
        /// 增加界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="depth">界面组深度。</param>
        /// <returns>是否增加界面组成功。</returns>
        public bool AddUIGroup(UIGroupEnum uiGroupName, int depth)
        {
            if (_uiManager.HasUIGroup(uiGroupName))
            {
                return false;
            }

            DefaultUIGroupHelper uiGroupHelper =
                Helper.CreateHelper<DefaultUIGroupHelper>(_uiGroupHelperTypeName, UIGroupCount);
            if (uiGroupHelper == null)
            {
                Log.Error("Can not create UI group helper.");
                return false;
            }

            uiGroupHelper.name = Utility.Text.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            uiGroupHelper.Init();
            Transform transform = uiGroupHelper.transform;
            transform.SetParent(_instanceRoot);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;

            return _uiManager.AddUIGroup(uiGroupName, depth, uiGroupHelper);
        }

        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUI(int serialId)
        {
            return _uiManager.HasUI(serialId);
        }

        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUI(string uiFormAssetName)
        {
            return _uiManager.HasUI(uiFormAssetName);
        }

        /// <summary>
        /// 异步打开UI页面
        /// </summary>
        /// <typeparam name="TViewModel">页面控制模块</typeparam>
        /// <returns>页面唯一Id</returns>
        public async UniTask<int> OpenUI<TViewModel>(UICtorInfo ctorInfo) where TViewModel : UIViewModelBase, new()
        {
            return await _uiManager.OpenUI<TViewModel>(ctorInfo);
        }

        /// <summary>
        /// 异步开启页面
        /// </summary>
        /// <param name="ctorInfo">页面配置</param>
        /// <param name="viewModel">页面控制模块</param>
        /// <returns>页面位移Id</returns>
        public async UniTask<int> OpenUI(UICtorInfo ctorInfo, UIViewModelBase viewModel)
        {
            return await _uiManager.OpenUI(ctorInfo, viewModel);
        }


        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>要获取的界面。</returns>
        public T GetUI<T>(int serialId) where T : IUIForm
        {
            return _uiManager.GetUI<T>(serialId);
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public T GetUI<T>(string uiFormAssetName) where T : IUIForm
        {
            return _uiManager.GetUI<T>(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public T[] GetAllUI<T>(string uiFormAssetName) where T : IUIForm
        {
            IUIForm[] uiForms = _uiManager.GetAllUI(uiFormAssetName);
            T[] uiFormImpls = new T[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                uiFormImpls[i] = (T)uiForms[i];
            }

            return uiFormImpls;
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        public void GetAllUI<T>(string uiFormAssetName, List<T> results) where T : IUIForm
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            _uiManager.GetAllUI(uiFormAssetName, _internalUIFormResults);
            foreach (IUIForm uiForm in _internalUIFormResults)
            {
                results.Add((T)uiForm);
            }
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <returns>所有已加载的界面。</returns>
        public T[] GetAllLoadedUI<T>() where T : IUIForm
        {
            IUIForm[] uiForms = _uiManager.GetAllLoadedUI();
            T[] uiFormImpls = new T[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                uiFormImpls[i] = (T)uiForms[i];
            }

            return uiFormImpls;
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <param name="results">所有已加载的界面。</param>
        public void GetAllLoadedUIForms<T>(List<T> results) where T : IUIForm
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            _uiManager.GetAllLoadedUI(_internalUIFormResults);
            foreach (T uiForm in _internalUIFormResults)
            {
                results.Add((T)uiForm);
            }
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <returns>所有正在加载界面的序列编号。</returns>
        public int[] GetAllLoadingUISerialIds()
        {
            return _uiManager.GetAllLoadingUISerialIds();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号。</param>
        public void GetAllLoadingUISerialIds(List<int> results)
        {
            _uiManager.GetAllLoadingUISerialIds(results);
        }

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否正在加载界面。</returns>
        public bool IsLoadingUI(int serialId)
        {
            return _uiManager.IsLoadingUI(serialId);
        }

        /// <summary>
        /// 是否是合法的界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        /// <returns>界面是否合法。</returns>
        public bool IsValidUI(IUIForm uiForm)
        {
            return _uiManager.IsValidUI(uiForm);
        }

        public void CloseUI(int serialId)
        {
            _uiManager.CloseUI(serialId);
        }

        public void CloseUI(IUIForm uiForm)
        {
            _uiManager.CloseUI(uiForm);
        }

        public void CloseUI(IUIForm uiForm, object userData)
        {
            _uiManager.CloseUI(uiForm, userData);
        }

        public void CloseAllLoadedUI()
        {
            _uiManager.CloseAllLoadedUI();
        }

        public void CloseAllLoadingUI()
        {
            _uiManager.CloseAllLoadingUI();
        }

        public void RefocusUI(IUIForm uiform)
        {
            _uiManager.RefocusUI(uiform);
        }

        /// <summary>
        /// 设置界面是否被加锁。
        /// </summary>
        /// <param name="uiForm">要设置是否被加锁的界面。</param>
        /// <param name="locked">界面是否被加锁。</param>
        public void SetUIInstanceLocked(IUIForm uiForm, bool locked)
        {
            if (uiForm == null)
            {
                Log.Warning("UI form is invalid.");
                return;
            }

            _uiManager.SetUIInstanceLocked(((MonoBehaviour)uiForm.Handle).gameObject, locked);
        }
    }
}
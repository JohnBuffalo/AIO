using System;
using System.Collections.Generic;
using AIOFramework.Event;
using AIOFramework.ObjectPool;
using AIOFramework.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AIOFramework.UI
{
    /// <summary>
    /// 流程组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("AIOFramework/UI")]
    public partial class UIComponent : GameFrameworkComponent
    {
        IUIManager m_UIManager;

        [SerializeField] private Transform m_InstanceRoot = null;
        [SerializeField] private string m_UIGroupHelperTypeName = "AIOFramework.Runtime.DefaultUIGroupHelper";
        // [SerializeField] private UIGroupDisplay[] m_UIGroups = null;
        private readonly List<IUIForm> m_InternalUIFormResults = new List<IUIForm>();
        [SerializeField] private float m_InstanceAutoReleaseInterval = 60f;
        [SerializeField] private int m_InstanceCapacity = 16;
        [SerializeField] private float m_InstanceExpireTime = 60f;

        public IUIManager UIManager {
            set {m_UIManager = value;}
            get{return m_UIManager;}
        }

        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        public int UIGroupCount
        {
            get { return m_UIManager.UIGroupCount; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get { return m_UIManager.InstanceAutoReleaseInterval; }
            set { m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        public int InstanceCapacity
        {
            get { return m_UIManager.InstanceCapacity; }
            set { m_UIManager.InstanceCapacity = m_InstanceCapacity = value; }
        }


        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        public float InstanceExpireTime
        {
            get { return m_UIManager.InstanceExpireTime; }
            set { m_UIManager.InstanceExpireTime = m_InstanceExpireTime = value; }
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
            m_UIManager = GameFrameworkEntry.GetModule(uiManagerType) as IUIManager;
            if (m_UIManager == null)
            {
                Log.Fatal("UI manager is null.");
                return;
            }
            
            m_UIManager.SetAssetManager(Entrance.Resource);
            m_UIManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
            m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
            m_UIManager.InstanceCapacity = m_InstanceCapacity;
            m_UIManager.InstanceExpireTime = m_InstanceExpireTime;
            m_UIManager.OpenUIFormSuccess += OnOpenUIFormSuccess;
            m_UIManager.OpenUIFormFailure += OnOpenUIFormFailure;
            m_UIManager.CloseUIFormComplete += OnCloseUIFormComplete;
            
            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = insRoot;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            m_InstanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");

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
            return m_UIManager.HasUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>要获取的界面组。</returns>
        public IUIGroup GetUIGroup(UIGroupEnum uiGroupName)
        {
            return m_UIManager.GetUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取所有界面组。
        /// </summary>
        /// <returns>所有界面组。</returns>
        public IUIGroup[] GetAllUIGroups()
        {
            return m_UIManager.GetAllUIGroups();
        }

        /// <summary>
        /// 获取所有界面组。
        /// </summary>
        /// <param name="results">所有界面组。</param>
        public void GetAllUIGroups(List<IUIGroup> results)
        {
            m_UIManager.GetAllUIGroups(results);
        }
        
        /// <summary>
        /// 增加界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="depth">界面组深度。</param>
        /// <returns>是否增加界面组成功。</returns>
        public bool AddUIGroup(UIGroupEnum uiGroupName, int depth)
        {
            if (m_UIManager.HasUIGroup(uiGroupName))
            {
                return false;
            }

            DefaultUIGroupHelper uiGroupHelper =
                Helper.CreateHelper<DefaultUIGroupHelper>(m_UIGroupHelperTypeName, UIGroupCount);
            if (uiGroupHelper == null)
            {
                Log.Error("Can not create UI group helper.");
                return false;
            }

            uiGroupHelper.name = Utility.Text.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            Transform transform = uiGroupHelper.transform;
            transform.SetParent(m_InstanceRoot);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;

            return m_UIManager.AddUIGroup(uiGroupName, depth, uiGroupHelper);
        }

        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUI(int serialId)
        {
            return m_UIManager.HasUI(serialId);
        }

        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>是否存在界面。</returns>
        public bool HasUI(string uiFormAssetName)
        {
            return m_UIManager.HasUI(uiFormAssetName);
        }

        /// <summary>
        /// 异步打开UI页面
        /// </summary>
        /// <param name="userData">页面初始参数</param>
        /// <typeparam name="TUICtorInfo">页面配置</typeparam>
        /// <typeparam name="TViewModel">页面控制模块</typeparam>
        /// <returns>页面唯一Id</returns>
        public async UniTask<int> OpenUI<TUICtorInfo, TViewModel>(object userData = null)
            where TUICtorInfo : UICtorInfo, new() where TViewModel : UIViewModelBase, new()
        {
            return await m_UIManager.OpenUI<TUICtorInfo, TViewModel>(userData);
        }

        /// <summary>
        /// 异步开启页面
        /// </summary>
        /// <param name="ctorInfo">页面配置</param>
        /// <param name="viewModel">页面控制模块</param>
        /// <param name="userData">页面初始化数据</param>
        /// <returns>页面位移Id</returns>
        public async UniTask<int> OpenUI(UICtorInfo ctorInfo, UIViewModelBase viewModel, object userData)
        {
            return await m_UIManager.OpenUI(ctorInfo, viewModel, userData);
        }
        
        
        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>要获取的界面。</returns>
        public T GetUI<T>(int serialId) where T:IUIForm
        {
            return m_UIManager.GetUI<T>(serialId);
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public T GetUI<T>(string uiFormAssetName) where T:IUIForm
        {
            return m_UIManager.GetUI<T>(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>要获取的界面。</returns>
        public T[] GetAllUI<T>(string uiFormAssetName) where T:IUIForm
        {
            IUIForm[] uiForms = m_UIManager.GetAllUI(uiFormAssetName);
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
        public void GetAllUI<T>(string uiFormAssetName, List<T> results) where T:IUIForm
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetAllUI(uiFormAssetName, m_InternalUIFormResults);
            foreach (IUIForm uiForm in m_InternalUIFormResults)
            {
                results.Add((T)uiForm);
            }
        }

        /// <summary>
        /// 获取所有已加载的界面。
        /// </summary>
        /// <returns>所有已加载的界面。</returns>
        public T[] GetAllLoadedUI<T>() where T:IUIForm
        {
            IUIForm[] uiForms = m_UIManager.GetAllLoadedUI();
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
        public void GetAllLoadedUIForms<T>(List<T> results)where T:IUIForm
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetAllLoadedUI(m_InternalUIFormResults);
            foreach (T uiForm in m_InternalUIFormResults)
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
            return m_UIManager.GetAllLoadingUISerialIds();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号。
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号。</param>
        public void GetAllLoadingUISerialIds(List<int> results)
        {
            m_UIManager.GetAllLoadingUISerialIds(results);
        }

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>是否正在加载界面。</returns>
        public bool IsLoadingUI(int serialId)
        {
            return m_UIManager.IsLoadingUI(serialId);
        }

        /// <summary>
        /// 是否是合法的界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        /// <returns>界面是否合法。</returns>
        public bool IsValidUI(IUIForm uiForm)
        {
            return m_UIManager.IsValidUI(uiForm);
        }
        
        public void CloseUI(int serialId)
        {
            m_UIManager.CloseUI(serialId);
        }

        public void CloseUI(IUIForm uiForm)
        {
            m_UIManager.CloseUI(uiForm);
        }

        public void CloseUI(IUIForm uiForm, object userData)
        {
            m_UIManager.CloseUI(uiForm, userData);
        }

        public void CloseAllLoadedUI()
        {
            m_UIManager.CloseAllLoadedUI();
        }

        public void CloseAllLoadingUI()
        {
            m_UIManager.CloseAllLoadingUI();
        }

        public void RefocusUI(IUIForm uiform)
        {
            m_UIManager.RefocusUI(uiform);
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

            m_UIManager.SetUIInstanceLocked(((MonoBehaviour)uiForm.Handle).gameObject, locked);
        }
    }
}
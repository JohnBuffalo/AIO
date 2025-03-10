using Loxodon.Framework.Binding;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Variables;

namespace AIOFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class UIViewBase : UIView, IUIForm
    {
        /// <summary>
        /// 页面上的组件集合
        /// </summary>
        [SerializeField] private VariableArray m_Variables;

        /// <summary>
        /// 页面UID
        /// </summary>
        private int m_SerialId;

        /// <summary>
        /// 页面资源名,默认是预制体名
        /// </summary>
        private string m_AssetName;

        /// <summary>
        /// 页面所属Group
        /// </summary>
        private IUIGroup m_UIGroup;

        /// <summary>
        /// 页面在组中层级
        /// </summary>
        private int m_DepthInUIGroup;

        /// <summary>
        /// 是否暂停下方页面
        /// </summary>
        private bool m_PauseCoveredUI;

        /// <summary>
        /// 页面Canvas
        /// </summary>
        private Canvas m_Canvas;

        /// <summary>
        /// 页面数据
        /// </summary>
        private object m_UserData;

        public UIViewModelBase ViewModel
        {
            get { return this.GetDataContext() as UIViewModelBase; }
            set { this.SetDataContext(value); }
        }
        
        public VariableArray Variables => m_Variables;

        public string Location { get; set; }
        public UIGroupEnum Group { get; set; }
        public int SerialId => m_SerialId;
        public string UIAssetName => m_AssetName;
        public object Handle => gameObject;

        public IUIGroup UIGroup
        {
            set { m_UIGroup = value; }
            get { return m_UIGroup; }
        }

        public bool Paused { get; set; }
        public bool Covered { get; set; }
        public int DepthInUIGroup
        {
            private set
            {
                m_DepthInUIGroup = value;
                Canvas.sortingOrder = m_DepthInUIGroup;
            }
            get => m_DepthInUIGroup;
        }

        public bool PauseCoveredUI => m_PauseCoveredUI;

        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponent<Canvas>();
                    m_Canvas.overrideSorting = true;
                }

                return m_Canvas;
            }
        }

        public virtual void OnInit(int serialId, string uiAssetName, IUIGroup uiGroup, UIViewModelBase viewModel, UICtorInfo ctorInfo)
        {
            m_SerialId = serialId;
            m_AssetName = uiAssetName;
            m_UIGroup = uiGroup;
            ViewModel = viewModel;
            m_PauseCoveredUI = ctorInfo.PauseCoveredUI;
        }

        public virtual void OnRecycle()
        {
            Log.Info($"{gameObject.name} OnRecycle");
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnOpen(object userData)
        {
            m_UserData = userData;
            SetActive(true);
        }

        /// <summary>
        /// 被关闭时候调用
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnClose(object userData)
        {
            Log.Info($"{UIAssetName} OnClose");
        }

        public virtual void OnPause()
        {
            Log.Info($"{gameObject.name} OnPause and Hide: {Covered.ToString()}");
            if (Covered)
            {
                SetActive(false);
            }
        }

        /// <summary>
        /// 从Pause状态恢复
        /// </summary>
        public virtual void OnResume()
        {
            Log.Info($"{gameObject.name} OnResume");
        }

        /// <summary>
        /// 当页面上方加入其他页面时触发
        /// </summary>
        /// <param name="pause">是否需要暂停页面</param>
        public virtual void OnCover()
        {
            Log.Info($"{gameObject.name} OnCover and Hide: {Paused.ToString()}");
            //当被覆盖,且需要暂停时隐藏页面. 通常是被全屏页面遮挡时触发
            if (Paused)
            {
                SetActive(false);
            }
        }

        /// <summary>
        /// 从被Cover状态恢复
        /// </summary>
        public virtual void OnReveal()
        {
            Log.Info($"{gameObject.name} OnReveal");
            SetActive(true);
        }

        public virtual void OnRefocus(object userData)
        {
            Log.Info($"{gameObject.name} OnRefocus");
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
            Log.Info($" {gameObject.name} OnDepthChanged uiGroupDepth:{uiGroupDepth} , depthInUIGroup:{depthInUIGroup}");
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            ReferencePool.Release(ViewModel);
            ViewModel = null;
        }

        protected T GetVariable<T>(string variableName)
        {
            return Variables.Get<T>(variableName);
        }

        public T GetViewModel<T>() where T : UIViewModelBase
        {
            return ViewModel as T;
        }

        public void SetActive(bool active)
        {
            if (active && !IsActive())
            {
                gameObject.SetActive(true);
            }else if (!active && IsActive())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
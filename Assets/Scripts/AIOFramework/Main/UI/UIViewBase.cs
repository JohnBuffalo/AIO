using System.Collections.Generic;
using Loxodon.Framework.Binding;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Variables;
using AIOFramework.Runtime;
using UnityEngine.Serialization;

namespace AIOFramework.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class UIViewBase : UIView, IUIForm
    {
        /// <summary>
        /// 页面上的组件集合
        /// </summary>
        [FormerlySerializedAs("variables")] [SerializeField]
        private VariableArray _variables;
        
        /// <summary>
        /// 页面UID
        /// </summary>
        private int _serialId;

        /// <summary>
        /// 页面资源名,默认是预制体名
        /// </summary>
        private string _assetName;

        /// <summary>
        /// 页面所属Group
        /// </summary>
        private IUIGroup _uiGroup;

        /// <summary>
        /// 页面在组中层级
        /// </summary>
        private int _depthInUIGroup;

        /// <summary>
        /// 是否暂停下方页面
        /// </summary>
        private bool _pauseCoveredUI;

        /// <summary>
        /// 页面Canvas
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// 页面数据
        /// </summary>
        private UICtorInfo _ctorInfo;

        public UICtorInfo CtorInfo => _ctorInfo;

        public UIViewModelBase ViewModel
        {
            get { return this.GetDataContext() as UIViewModelBase; }
            set { this.SetDataContext(value); }
        }

        public VariableArray Variables => _variables;
        public string Location { get; set; }
        public UIGroupEnum Group { get; set; }
        public int SerialId => _serialId;
        public string UIAssetName => _assetName;
        public object Handle => gameObject;

        public IUIGroup UIGroup
        {
            set { _uiGroup = value; }
            get { return _uiGroup; }
        }

        public bool Paused { get; set; }
        public bool Covered { get; set; }

        public int DepthInUIGroup
        {
            private set
            {
                _depthInUIGroup = value;
                Canvas.sortingOrder = _depthInUIGroup;
            }
            get => _depthInUIGroup;
        }

        public bool PauseCoveredUI => _pauseCoveredUI;

        public Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                    _canvas.overrideSorting = true;
                }

                return _canvas;
            }
        }

        public virtual void OnInit(int serialId, string uiAssetName, IUIGroup uiGroup, UIViewModelBase viewModel,
            UICtorInfo ctorInfo)
        {
            this._serialId = serialId;
            _assetName = uiAssetName;
            this._uiGroup = uiGroup;
            ViewModel = viewModel;
            this._ctorInfo = ctorInfo;
            _pauseCoveredUI = ctorInfo.PauseCoveredUI;
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
            Log.Info($"{UIAssetName} OnOpen");
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
            Log.Info($"{gameObject.name} OnPause ");
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

        /// <summary>
        /// 从低位跃迁至顶部显示
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnRefocus(object userData)
        {
            Log.Info($"{gameObject.name} OnRefocus");
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            // Log.Info($"{SerialId} OnUpdate");
        }

        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
            Log.Info(
                $" {gameObject.name} OnDepthChanged uiGroupDepth:{uiGroupDepth} , depthInUIGroup:{depthInUIGroup}");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ReferencePool.Release(ViewModel);
            ReferencePool.Release(CtorInfo);
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
            }
            else if (!active && IsActive())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
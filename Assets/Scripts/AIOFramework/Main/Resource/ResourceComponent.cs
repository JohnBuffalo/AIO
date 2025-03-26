using System.Collections.Generic;
using AIOFramework.Runtime;
using UnityEngine;
using YooAsset;

namespace AIOFramework.Resource
{
    [DisallowMultipleComponent]
    [AddComponentMenu("AIOFramework/Resource")]
    public partial class ResourceComponent : GameFrameworkComponent
    {
        [SerializeField] 
        private string _packageName = "DefaultPackage";
        public string PackageName => _packageName;
        
        [SerializeField] 
        private EPlayMode _playMode = EPlayMode.EditorSimulateMode;
        public EPlayMode PlayMode
        {
            get
            {
#if UNITY_EDITOR
                return _playMode;
#elif UNITY_WEBGL
                return EPlayMode.WebPlayMode;
#else
                return EPlayMode.HostPlayMode;
#endif
            }
        }

        [SerializeField]
        [Tooltip("异步操作每帧最大时间切片(毫秒)")]
        private long _timeSlice = 1000L;
        public long TimeSlice => _timeSlice;
        
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
    }
}
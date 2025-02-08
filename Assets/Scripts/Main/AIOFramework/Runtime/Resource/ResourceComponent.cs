using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace AIO.Framework.Runtime
{
    public class ResourceComponent : GameFrameworkComponent
    {
        [SerializeField] 
        private string PackageName = "DefaultPackage";

        private EPlayMode playMode = EPlayMode.EditorSimulateMode;

        public EPlayMode PlayMode
        {
            get
            {
#if UNITY_EDITOR
                return playMode;
#elif UNITY_WEBGL
                return EPlayMode.WebPlayMode;
#else
                return EPlayMode.HostPlayMode;
#endif
            }
        }
        
        private ResourcePackage m_ResourcePackage;
        public ResourcePackage ResourcePackage
        {
            get
            {
                if (m_ResourcePackage == null)
                {
                    m_ResourcePackage = YooAssets.GetPackage(PackageName);
                }

                return m_ResourcePackage;
            }
        }
        
        /// <summary>
        /// 资源句柄集合
        /// </summary>
        private readonly Dictionary<string, HandleBase> m_Handles = new();
        public Dictionary<string, HandleBase> Handles => m_Handles;


        protected override void Awake()
        {
            base.Awake();
        }
    }
}
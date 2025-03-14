using System;
using UnityEngine;

public enum ServerTypeEnum
{
    /// <summary>
    /// 本地宿主机
    /// </summary>
    Local = 0,

    /// <summary>
    /// 内网
    /// </summary>
    Intranet = 1,

    /// <summary>
    /// 外网
    /// </summary>
    Extranet = 2,

    /// <summary>
    /// 正式服
    /// </summary>
    Formal = 3
}

namespace AIOFramework.Setting
{
    [Serializable]
    public class GameSetting
    {
        [SerializeField] private ServerTypeEnum m_ServerType = ServerTypeEnum.Intranet;

        public ServerTypeEnum ServerType => m_ServerType;

        [SerializeField] private string m_Version = "0.0.0";
        public string Version { get => m_Version; set => m_Version = value; }

        [Tooltip("是否在构建资源的时候清理上传到服务端目录的老资源")] [SerializeField]
        private bool m_CleanCommitPathRes = true;

        public bool CleanCommitPathRes => m_CleanCommitPathRes;

        
        [Tooltip("宿主机资源地址")] [SerializeField]
        private string m_LocalResourceUrl = "http://127.0.0.1";
        public string LocalResourceUrl => m_LocalResourceUrl;
        [Tooltip("Dev内网资源地址")] [SerializeField]
        private string m_InnerResourceUrl = "http://127.0.0.1";
        public string InnerResourceUrl => m_InnerResourceUrl;

        [Tooltip("Dev外网资源地址")] [SerializeField]
        private string m_ExtraResourceUrl = "http://127.0.0.1";

        public string ExtraResourceUrl => m_ExtraResourceUrl;


        [Tooltip("Master线上资源地址")] [SerializeField]
        private string m_FormalResourceUrl = "http://127.0.0.1";

        public string FormalResourceUrl => m_FormalResourceUrl;

        [Tooltip("本地资源服务器地址")][SerializeField]
        private string m_LocalServerDirectory = "D:/UnityReferences/AIOSimulateServer";
        public string LocalServerDirectory => m_LocalServerDirectory;

        [Tooltip("热更程序集Dll存放路径")][SerializeField]
        private string m_HotUpdateDllDirectory = "ArtAssets/HotUpdate";
        public string HotUpdateDllDirectory => m_HotUpdateDllDirectory;
    }
}
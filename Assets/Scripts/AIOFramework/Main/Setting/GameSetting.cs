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
        [SerializeField] 
        private ServerTypeEnum _serverType = ServerTypeEnum.Local;

        public ServerTypeEnum ServerType => _serverType;

        [SerializeField] private string _version = "0.0.0";
        public string Version { get => _version; set => _version = value; }

        [Tooltip("是否在构建资源的时候清理上传到服务端目录的老资源")] [SerializeField]
        private bool _cleanCommitPathRes = true;
        public bool CleanCommitPathRes => _cleanCommitPathRes;

        
        [Tooltip("宿主机资源地址")] [SerializeField]
        private string _localResourceUrl = "http://127.0.0.1";
        public string LocalResourceUrl => _localResourceUrl;
        [Tooltip("Dev内网资源地址")] [SerializeField]
        private string _innerResourceUrl = "http://127.0.0.1";
        public string InnerResourceUrl => _innerResourceUrl;

        [Tooltip("Dev外网资源地址")] [SerializeField]
        private string _extraResourceUrl = "http://127.0.0.1";

        public string ExtraResourceUrl => _extraResourceUrl;

        [Tooltip("Master线上资源地址")] [SerializeField]
        private string _formalResourceUrl = "http://127.0.0.1";

        public string FormalResourceUrl => _formalResourceUrl;

        [Tooltip("本地资源服务器地址")][SerializeField]
        private string _localServerDirectory = "D:/UnityReferences/AIOHttpsServer";
        public string LocalServerDirectory => _localServerDirectory;

        [Tooltip("热更程序集Dll存放路径")][SerializeField]
        private string _hotUpdateDllDirectory = "ArtAssets/HotUpdate";
        public string HotUpdateDllDirectory => _hotUpdateDllDirectory;
    }
}
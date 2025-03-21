using System.Collections.Generic;
using YooAsset;

namespace AIOFramework.UI
{
    /// <summary>
    /// UI内部加载资源接口
    /// </summary>
    interface IUILoadProxy
    {
        List<HandleBase> Handles { get; set;}
        void Load(string location);
        void OnDispose();
    }
}
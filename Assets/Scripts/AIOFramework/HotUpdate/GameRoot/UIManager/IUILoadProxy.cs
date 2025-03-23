using System.Collections.Generic;
using YooAsset;

namespace AIOFramework.UI
{
    /// <summary>
    /// UI内部加载资源接口
    /// </summary>
    interface IUILoadProxy
    {
        string CurLocation{get;set;}
        List<HandleBase> Handles { get; set;}
        void OnLocationChange();
        void OnDispose();
    }
}
﻿
using AIOFramework.Runtime;

namespace AIOFramework.UI
{
    /// <summary>
    /// UIView构造参数
    /// </summary>
    public abstract class UICtorInfo : IReference
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public abstract string Location {get;}
        /// <summary>
        /// 页面组
        /// </summary>
        public abstract UIGroupEnum Group { get; }
        /// <summary>
        /// 是否暂停和隐藏下方页面.**全屏页面建议设置为true
        /// </summary>
        public abstract bool PauseCoveredUI { get; }

        /// <summary>
        /// 允许开启多个.**除了提示弹窗都应该设置为false
        /// </summary>
        public virtual bool Multiple => false;

        //注意: 会通过引用池复用,只清除子类中自定义的参数
        public virtual void Clear()
        {
            Log.Info("Clearing UI ctor info");
        }

        public virtual string AssetName => Utility.Path.GetFileNameWithoutExtension(Location);
    }
}
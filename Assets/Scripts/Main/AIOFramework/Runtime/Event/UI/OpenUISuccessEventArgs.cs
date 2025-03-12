using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 打开界面成功事件。
    /// </summary>
    public sealed class OpenUISuccessEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OpenUISuccessEventArgs).GetHashCode();
        public override int Id =>EventId;

        /// <summary>
        /// 初始化打开界面成功事件的新实例。
        /// </summary>
        public OpenUISuccessEventArgs()
        {
            UIView = null;
            UserData = null;
        }

        /// <summary>
        /// 获取打开成功的界面。
        /// </summary>
        public IUIForm UIView
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建打开界面成功事件。
        /// </summary>
        /// <param name="uiView">加载成功的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的打开界面成功事件。</returns>
        public static OpenUISuccessEventArgs Create(IUIForm uiView , object userData)
        {
            OpenUISuccessEventArgs openUISuccessEventArgs = ReferencePool.Acquire<OpenUISuccessEventArgs>();
            openUISuccessEventArgs.UIView = uiView;
            openUISuccessEventArgs.UserData = userData;
            return openUISuccessEventArgs;
        }

        /// <summary>
        /// 清理打开界面成功事件。
        /// </summary>
        public override void Clear()
        {
            UIView = null;
            UserData = null;
        }

    }
}

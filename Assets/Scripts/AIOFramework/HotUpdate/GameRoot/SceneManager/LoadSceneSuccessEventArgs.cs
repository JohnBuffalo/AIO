
using AIOFramework.Runtime;
using YooAsset;

namespace AIOFramework.Event
{
    /// <summary>
    /// 加载场景成功事件。
    /// </summary>
    public sealed class LoadSceneSuccessEventArgs : BaseEventArgs
    {
        /// <summary>
        /// 初始化加载场景成功事件的新实例。
        /// </summary>
        public LoadSceneSuccessEventArgs()
        {
            SceneAssetName = null;
            Handle = null;
            UserData = null;
        }

        /// <summary>
        /// 获取场景资源名称。
        /// </summary>
        public string SceneAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取加载持续时间。
        /// </summary>
        public SceneHandle Handle
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
        public static readonly int s_EventId = typeof(LoadSceneSuccessEventArgs).GetHashCode();
        public override int Id => s_EventId;

        /// <summary>
        /// 创建加载场景成功事件。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="handle">SceneHandle</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的加载场景成功事件。</returns>
        public static LoadSceneSuccessEventArgs Create(string sceneAssetName, SceneHandle handle, object userData)
        {
            LoadSceneSuccessEventArgs loadSceneSuccessEventArgs = ReferencePool.Acquire<LoadSceneSuccessEventArgs>();
            loadSceneSuccessEventArgs.SceneAssetName = sceneAssetName;
            loadSceneSuccessEventArgs.Handle = handle;
            loadSceneSuccessEventArgs.UserData = userData;
            return loadSceneSuccessEventArgs;
        }

        /// <summary>
        /// 清理加载场景成功事件。
        /// </summary>
        public override void Clear()
        {
            SceneAssetName = null;
            Handle = null;
            UserData = null;
        }
    }
}

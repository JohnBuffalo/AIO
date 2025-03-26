//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using AIOFramework.Runtime;

namespace AIOFramework.Event
{
    /// <summary>
    /// 激活场景被改变事件。
    /// </summary>
    public sealed class ActiveSceneChangedEventArgs : BaseEventArgs
    {
        /// <summary>
        /// 激活场景被改变事件编号。
        /// </summary>
        public static readonly int s_EventId = typeof(ActiveSceneChangedEventArgs).GetHashCode();

        /// <summary>
        /// 初始化激活场景被改变事件的新实例。
        /// </summary>
        public ActiveSceneChangedEventArgs()
        {
            LastActiveScene = default;
            ActiveScene = default;
        }

        /// <summary>
        /// 获取激活场景被改变事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return s_EventId;
            }
        }

        /// <summary>
        /// 获取上一个被激活的场景。
        /// </summary>
        public SceneProxy LastActiveScene
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取被激活的场景。
        /// </summary>
        public SceneProxy ActiveScene
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建激活场景被改变事件。
        /// </summary>
        /// <param name="lastActiveScene">上一个被激活的场景。</param>
        /// <param name="activeScene">被激活的场景。</param>
        /// <returns>创建的激活场景被改变事件。</returns>
        public static ActiveSceneChangedEventArgs Create(SceneProxy lastActiveScene, SceneProxy activeScene)
        {
            ActiveSceneChangedEventArgs activeSceneChangedEventArgs = ReferencePool.Acquire<ActiveSceneChangedEventArgs>();
            activeSceneChangedEventArgs.LastActiveScene = lastActiveScene;
            activeSceneChangedEventArgs.ActiveScene = activeScene;
            return activeSceneChangedEventArgs;
        }

        /// <summary>
        /// 清理激活场景被改变事件。
        /// </summary>
        public override void Clear()
        {
            LastActiveScene = default;
            ActiveScene = default;
        }
    }
}

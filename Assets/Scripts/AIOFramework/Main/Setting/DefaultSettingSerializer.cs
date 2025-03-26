﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using AIOFramework.Runtime;

namespace AIOFramework.Setting
{
    /// <summary>
    /// 默认游戏配置序列化器。
    /// </summary>
    public sealed class DefaultSettingSerializer : GameFrameworkSerializer<DefaultSetting>
    {
        private static readonly byte[] s_header = new byte[] { (byte)'G', (byte)'F', (byte)'S' };

        /// <summary>
        /// 初始化默认游戏配置序列化器的新实例。
        /// </summary>
        public DefaultSettingSerializer()
        {
        }

        /// <summary>
        /// 获取默认游戏配置头标识。
        /// </summary>
        /// <returns>默认游戏配置头标识。</returns>
        protected override byte[] GetHeader()
        {
            return s_header;
        }
    }
}

using System;
using AIOFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace AIOFramework.UI
{
    [RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
    /// <summary>
    /// 默认界面组辅助器。
    /// </summary>
    public class DefaultUIGroupHelper : MonoBehaviour, IUIGroupHelper
    {
        Canvas m_Canvas;
        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = gameObject.GetOrAddComponent<Canvas>();
                    Canvas.overrideSorting = true;
                }
                return m_Canvas;
            }
        }
        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public void SetDepth(int depth)
        {
            Canvas.sortingOrder = depth;
        }
    }
}
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
        private Canvas _canvas;
        public Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = gameObject.GetOrAddComponent<Canvas>();
                    Canvas.overrideSorting = true;
                }
                return _canvas;
            }
        }

        public void Init()
        {
            var rectTrans = GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.sizeDelta = new Vector2(1080,1920);
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
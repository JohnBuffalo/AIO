using System;
using UnityEngine;

namespace AIOFramework.Runtime
{
    public class UIRoot: MonoBehaviour
    {
        private static UIRoot _instance;
        public static UIRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIRoot>();
                }
                return _instance;
            }
        }
        private Camera m_Camera;

        public Camera Camera
        {
            get
            {
                if (m_Camera == null)
                {
                    m_Camera = GetComponent<Camera>();
                }
                return m_Camera;
            }
        }
        
        private Canvas m_Canvas;

        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponent<Canvas>();
                }
                return m_Canvas;
            }   
        }

        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
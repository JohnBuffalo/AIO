using System;
using UnityEngine;

namespace AIOFramework.UI
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
        private Camera _camera;

        public Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = GetComponentInChildren<Camera>();
                }
                return _camera;
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
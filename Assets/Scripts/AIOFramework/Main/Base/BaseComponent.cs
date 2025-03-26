using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 基础组件,
    /// </summary>
    [DisallowMultipleComponent]
    public class BaseComponent : GameFrameworkComponent
    {
        private const int DefaultDpi = 96; // default windows dpi
        private float _gameSpeedBeforePause = 1f;
        [SerializeField] private int _frameRate = 30;
        [SerializeField] private float _gameSpeed = 1f;
        [SerializeField] private bool _runInBackground = true;
        [SerializeField] private bool _neverSleep = true;
        [SerializeField] private string _logHelperTypeName = "AIOFramework.Runtime.DefaultLogHelper";
        /// <summary>
        /// 获取或设置游戏帧率.
        /// </summary>
        public int FrameRate
        {
            get
            {
                return _frameRate;
            }
            set
            {
                Application.targetFrameRate = _frameRate = value;
            }
        }
        
        /// <summary>
        /// 获取或设置游戏速度。
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                Time.timeScale = _gameSpeed = value >= 0f ? value : 0f;
            }
        }
        
        /// <summary>
        /// 获取游戏是否暂停。
        /// </summary>
        public bool IsGamePaused
        {
            get
            {
                return _gameSpeed <= 0f;
            }
        }
        
        /// <summary>
        /// 获取是否正常游戏速度。
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get
            {
                return _gameSpeed == 1f;
            }
        }
        
        /// <summary>
        /// 获取或设置是否允许后台运行。
        /// </summary>
        public bool RunInBackground
        {
            get
            {
                return _runInBackground;
            }
            set
            {
                Application.runInBackground = _runInBackground = value;
            }
        }
        
        /// <summary>
        /// 获取或设置是否禁止休眠。
        /// </summary>
        public bool NeverSleep
        {
            get
            {
                return _neverSleep;
            }
            set
            {
                _neverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }
        
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // InitTextHelper();
            // InitVersionHelper();
            InitLogHelper();

#if UNITY_5_3_OR_NEWER || UNITY_5_3
            // InitCompressionHelper();
            // InitJsonHelper();

            Utility.Converter.ScreenDpi = Screen.dpi;
            if (Utility.Converter.ScreenDpi <= 0)
            {
                Utility.Converter.ScreenDpi = DefaultDpi;
            }

            Application.targetFrameRate = _frameRate;
            Time.timeScale = _gameSpeed;
            Application.runInBackground = _runInBackground;
            Screen.sleepTimeout = _neverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
#else
            Log.Error("Game Framework only applies with Unity 5.3 and above, but current Unity version is {0}.", Application.unityVersion);
            GameEntry.Shutdown(ShutdownType.Quit);
#endif
#if UNITY_5_6_OR_NEWER
            Application.lowMemory += OnLowMemory;
#endif
            DontDestroyOnLoad(this);
        }
        
        private void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        private void OnApplicationQuit()
        {
#if UNITY_5_6_OR_NEWER
            Application.lowMemory -= OnLowMemory;
#endif
            StopAllCoroutines();
        }
        
        private void OnDestroy()
        {
            GameFrameworkEntry.Shutdown();
        }
        
        /// <summary>
        /// 暂停游戏。
        /// </summary>
        public void PauseGame()
        {
            if (IsGamePaused)
            {
                return;
            }

            _gameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }
        
        /// <summary>
        /// 恢复游戏。
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
            {
                return;
            }

            GameSpeed = _gameSpeedBeforePause;
        }
        
        /// <summary>
        /// 重置为正常游戏速度。
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed)
            {
                return;
            }

            GameSpeed = 1f;
        }
        
        internal void Shutdown()
        {
            Destroy(gameObject);
        }
        
        private void OnLowMemory()
        {
            Log.Info("Low memory reported...");

            // ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            // if (objectPoolComponent != null)
            // {
            //     objectPoolComponent.ReleaseAllUnused();
            // }

            // ResourceComponent resourceCompoent = GameEntry.GetComponent<ResourceComponent>();
            // if (resourceCompoent != null)
            // {
            //     resourceCompoent.ForceUnloadUnusedAssets(true);
            // }
        }
        
        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(_logHelperTypeName))
            {
                return;
            }

            Type logHelperType = Utility.Assembly.GetType(_logHelperTypeName);
            if (logHelperType == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not find log helper type '{0}'.", _logHelperTypeName));
            }

            GameFrameworkLog.ILogHelper logHelper = (GameFrameworkLog.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not create log helper instance '{0}'.", _logHelperTypeName));
            }

            GameFrameworkLog.SetLogHelper(logHelper);
        }
    }
}
﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Rendering;
#endif
using AIOFramework.Resource;
using AIOFramework.Setting;

namespace AIOFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class EnvironmentInformationWindow : ScrollableDebuggerWindowBase
        {
            private BaseComponent m_BaseComponent = null;
            private ResourceComponent m_ResourceComponent = null;

            public override void Initialize(params object[] args)
            {
                m_BaseComponent = GameEntry.GetComponent<BaseComponent>();
                if (m_BaseComponent == null)
                {
                    Log.Fatal("Base component is invalid.");
                    return;
                }

                m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
                if (m_ResourceComponent == null)
                {
                    Log.Fatal("Resource component is invalid.");
                    return;
                }
            }

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Environment Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Product Name", Application.productName);
                    DrawItem("Company Name", Application.companyName);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Game Identifier", Application.identifier);
#else
                    DrawItem("Game Identifier", Application.bundleIdentifier);
#endif
                    DrawItem("Game Version", Utility.Text.Format(" {0}", SettingUtility.GlobalSettings.GameSetting.Version));
                    DrawItem("Resource Version", Utility.Text.Format(" {0}",m_ResourceComponent.PlayMode));
                    DrawItem("Unity Version", Application.unityVersion);
                    DrawItem("Platform", Application.platform.ToString());
                    DrawItem("System Language", Application.systemLanguage.ToString());
                    DrawItem("Cloud Project Id", Application.cloudProjectId);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Build Guid", Application.buildGUID);
#endif
                    DrawItem("Target Frame Rate", Application.targetFrameRate.ToString());
                    DrawItem("Internet Reachability", Application.internetReachability.ToString());
                    DrawItem("Background Loading Priority", Application.backgroundLoadingPriority.ToString());
                    DrawItem("Is Playing", Application.isPlaying.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Splash Screen Is Finished", SplashScreen.isFinished.ToString());
#else
                    DrawItem("Is Showing Splash Screen", Application.isShowingSplashScreen.ToString());
#endif
                    DrawItem("Run In Background", Application.runInBackground.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Install Name", Application.installerName);
#endif
                    DrawItem("Install Mode", Application.installMode.ToString());
                    DrawItem("Sandbox Type", Application.sandboxType.ToString());
                    DrawItem("Is Mobile Platform", Application.isMobilePlatform.ToString());
                    DrawItem("Is Console Platform", Application.isConsolePlatform.ToString());
                    DrawItem("Is Editor", Application.isEditor.ToString());
                    DrawItem("Is Debug Build", Debug.isDebugBuild.ToString());
#if UNITY_5_6_OR_NEWER
                    DrawItem("Is Focused", Application.isFocused.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                    DrawItem("Is Batch Mode", Application.isBatchMode.ToString());
#endif
#if UNITY_5_3
                    DrawItem("Stack Trace Log Type", Application.stackTraceLogType.ToString());
#endif
                }
                GUILayout.EndVertical();
            }
        }
    }
}

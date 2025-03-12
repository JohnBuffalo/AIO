using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AIOFramework.Setting;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace AIOFramework.Editor.CI
{
    public partial class GameAppBuilder
    {
        /// <summary>
        /// 大版本出包
        /// </summary
        [MenuItem("GameBuilder/MajorVersion 大版本出包", priority = 0)]
        public static void MajorVersionBuild()
        {
            if (EditorUtility.DisplayDialog("提示", $"开始构建大版本,目标版本号 : {GetNextMajorVersion()}！", "Yes", "No"))
            {
                EditorTools.ClearUnityConsole();
                EditorApplication.delayCall += MajorVersionInternal;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }
        
        /// <summary>
        /// 小版本热更
        /// </summary>
        [MenuItem("GameBuilder/MinorVersion 小版本出包", priority = 1)]
        public static void MinorVersionBuild()
        {
            var packageName = AssetBundleCollectorSettingData.Setting.Packages[0].PackageName;
            var builtinPath = Application.streamingAssetsPath + $"/{YooAssetSettingsData.Setting.DefaultYooFolderName}/{packageName}";

            if (!Directory.Exists(builtinPath) || Directory.GetFiles(builtinPath).Length == 0)
            {
                Debug.LogError("请先构建大版本");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", $"开始构建热更版本,目标版本号 : {GetNextMinorVersion()}！", "Yes", "No"))
            {
                EditorTools.ClearUnityConsole();
                EditorApplication.delayCall += MinorVersionInternal;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }
        
        private static string GetNextMajorVersion()
        {
            var settings = SettingUtility.GlobalSettings.GameSetting;
            var version = settings.Version.Split('.');
            Debug.Log($"当前版本 : {settings.Version}");
            var year = version[0];
            int major = int.Parse(version[1]);
            var nextVersion = $"{year}.{major + 1}.{0}";
            return nextVersion;
        }
        
        private static string GetNextMinorVersion()
        {
            var settings = SettingUtility.GlobalSettings.GameSetting;
            var version = settings.Version.Split('.');
            Debug.Log($"当前版本 : {settings.Version}");
            var year = version[0];
            var major = version[1];
            var minor = int.Parse(version[2]);
            var nextVersion = $"{year}.{major}.{minor + 1}";
            return nextVersion;
        }

        /// <summary>
        /// 构建大版本
        /// </summary>
        private static void MajorVersionInternal()
        {
            var settings = SettingUtility.GlobalSettings.GameSetting;
            var nextVersion = GetNextMajorVersion();
            var buildPackageSuccess = BuildPackages(nextVersion, EBuildinFileCopyOption.ClearAndCopyAll);
            if (!buildPackageSuccess)
            {
                Debug.LogError("Build Package failed");
                return;
            }
            //生成app包体
            var buildAppSuccess = BuildApp();
            if (!buildAppSuccess) return;
            settings.Version = nextVersion;
            //资源发布
            CopyBundlesToCdn();
            AssetDatabase.SaveAssetIfDirty(SettingUtility.GlobalSettings);
            AssetDatabase.Refresh();
        }

        private static void MinorVersionInternal()
        {
            var settings = SettingUtility.GlobalSettings.GameSetting;
            var nextVersion = GetNextMinorVersion();
            
            var buildPackageSuccess = BuildPackages(nextVersion, EBuildinFileCopyOption.None);
            if (!buildPackageSuccess)
            {
                Debug.LogError("Build Package failed");
                return;
            }

            settings.Version = nextVersion;
            //资源发布
            CopyBundlesToCdn();
            AssetDatabase.SaveAssetIfDirty(SettingUtility.GlobalSettings);
            AssetDatabase.Refresh();
        }

        private static bool BuildPackages(string nextVersion, EBuildinFileCopyOption copyOption)
        {
            //1.编译C#代码
            PrebuildCommand.GenerateAll();
            //2.拷贝HotUpdate和AOT到指定目录
            CopyDllToAssets();
            //3.构建资源包
            AssetBundleCollectorSettingData.FixFile();
            AssetDatabase.Refresh();
            var buildBundleResult =
                RunBuildBundle_SBP(nextVersion, EditorUserBuildSettings.activeBuildTarget, copyOption);
            AssetDatabase.Refresh();
            if (!buildBundleResult)
            {
                Debug.LogError("Build Bundle Failed");
                return false;
            }

            return true;
        }

        private static bool BuildApp()
        {
            // 获取当前激活的平台
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildOptions options = BuildOptions.None;
            
            // 配置通用构建设置
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                Debug.LogError("No scenes in build settings!");
                return false;
            }

            // 平台特定配置
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, (int)Architecture.X64);
                    break;

                case BuildTarget.Android:
                    EditorUserBuildSettings.buildAppBundle = false; // 生成APK
                    PlayerSettings.Android.useCustomKeystore = true;
                    PlayerSettings.Android.keyaliasName = "your_alias";
                    PlayerSettings.Android.keystorePass = "keystore_password";
                    PlayerSettings.Android.keyaliasPass = "alias_password";
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
                    break;

                case BuildTarget.iOS:
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.yourcompany.yourapp");
                    PlayerSettings.iOS.appleEnableAutomaticSigning = false;
                    PlayerSettings.iOS.appleDeveloperTeamID = "TEAM_ID_HERE";
                    PlayerSettings.iOS.targetOSVersionString = "14.0";
                    break;
            }

            // 执行构建
            string outputPath = GetOutputPath(target);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
                locationPathName = outputPath,
                target = target,
                options = options
            });

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError($"Build failed: {report.summary}");
                return false;
            }

            Debug.Log($"Build succeeded: {outputPath}");
            return true;
        }
        
        private static string GetOutputPath(BuildTarget target)
        {
            string version = SettingUtility.GlobalSettings.GameSetting.Version;
            return target switch
            {
                BuildTarget.Android => $"Builds/Android/{version}/game_{version}.apk",
                BuildTarget.iOS => $"Builds/iOS/{version}/XcodeProject",
                BuildTarget.StandaloneWindows64 => $"Builds/Windows/{version}/Game_{version}.exe",
                _ => throw new ArgumentOutOfRangeException($"{target} is not supported")
            };
        }

        private static void CopyBundlesToCdn()
        {
            var settings = SettingUtility.GlobalSettings.GameSetting;
            var destDirectoryPath = YooAsset.PathUtility.RegularPath(GetCdnURL(SettingUtility.PlatformName()));
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var packageName = AssetBundleCollectorSettingData.Setting.Packages[0].PackageName;
            var srcDirectoryPath = YooAsset.PathUtility.RegularPath(Application.dataPath +
                                                                    $"/../Bundles/{buildTarget}/{packageName}/{settings.Version}");
            if (!Directory.Exists(destDirectoryPath))
            {
                Directory.CreateDirectory(destDirectoryPath);
            }

            if (!Directory.Exists(srcDirectoryPath))
            {
                Debug.LogError($"Source Directory is Invalid {srcDirectoryPath}");
                return;
            }
            
            var srcFiles = Directory.GetFiles(srcDirectoryPath);
            for (int i = 0; i < srcFiles.Length; i++)
            {
                var fileName = Path.GetFileName(srcFiles[i]);
                File.Copy(srcFiles[i], YooAsset.PathUtility.RegularPath($"{destDirectoryPath}/{fileName}"),
                    true);
            }
        }

        private static string GetCdnURL(string platform)
        {
            return PathUtility.RegularPath($"{SettingUtility.GlobalSettings.GameSetting.LocalServerDirectory}/{platform}");
            // var serverType = SettingUtility.GlobalSettings.GameSetting.ServerType;
            // switch (serverType)
            // {
            //     case ServerTypeEnum.Intranet:
            //         return SettingUtility.GlobalSettings.GameSetting.InnerResourceSourceUrl;
            //     case ServerTypeEnum.Extranet:
            //         return SettingUtility.GlobalSettings.GameSetting.ExtraResourceSourceUrl;
            //     case ServerTypeEnum.Formal:
            //         return SettingUtility.GlobalSettings.GameSetting.FormalResourceSourceUrl;  
            //     default:
            //         return PathUtility.RegularPath($"{SettingUtility.GlobalSettings.GameSetting.WindowServerDirectory}/{platform}");
            // }
        }
    }
}
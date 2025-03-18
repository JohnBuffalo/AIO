using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AIOFramework.Runtime;
using AIOFramework.Setting;
using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using YooAsset;

namespace HotUpdate
{
    public partial class Game : MonoBehaviour
    {
        private Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

        /// <summary>
        /// 来自AOTGenericReferences. 与HybridCLR.Setting面板中的Path AOT Assemblies保持一致
        /// </summary>
        private List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
            "UnityEngine.CoreModule.dll",
            "AIOFramework.Runtime.dll",
            "Loxodon.Framework.dll",
            "UniTask.dll",
        };

        private async UniTask LoadDlls()
        {
            await CacheAssembliesBytes();
            Log.Info("[LoadDlls] CacheAssemblies Finish");
            var list = new List<UniTask>();
            LoadMetadataForAOTAssemblies();
            Log.Info("[LoadDlls] LoadMetadataForAOTAssemblies Finish");
            s_assetDatas.Clear();
        }

        private async UniTask CacheAssembliesBytes()
        {
            var totalFileNames = AOTMetaAssemblyFiles;
            foreach (var fileName in totalFileNames)
            {
                await LoadAssemblyBytes(fileName);
            }
        }
        
        private byte[] ReadBytesFromCache(string dllName)
        {
            return s_assetDatas[dllName];
        }
        
        private async UniTask LoadAssemblyBytes(string fileName)
        {
            var dllDirectory = Path.Combine("Assets",
                SettingUtility.GlobalSettings.GameSetting.HotUpdateDllDirectory);
            var location = Utility.Path.GetRegularPath(Path.Combine(dllDirectory, fileName));

            var dllText = await Entrance.Resource.LoadAssetAsync<TextAsset>(location);
            s_assetDatas.Add(fileName, dllText.bytes);
            Log.Info($"Load {fileName}.bytes success");
            Log.Info("------------------------------------------------------------------");
        }

        private void LoadMetadataForAOTAssemblies()
        {
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            for (int i = 0; i < AOTMetaAssemblyFiles.Count; i++)
            {
                var bytes = ReadBytesFromCache(AOTMetaAssemblyFiles[i]);
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(bytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{AOTMetaAssemblyFiles[i]}. mode:{mode} ret:{err}");
            }
        }
    }
}
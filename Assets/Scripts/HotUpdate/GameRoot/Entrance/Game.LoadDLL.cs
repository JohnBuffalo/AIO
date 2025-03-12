using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AIOFramework.Runtime;
using AIOFramework.Setting;
using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;

namespace HotUpdate
{
    public partial class Game
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

        private List<string> HotUpdateAssemblyFiles { get; } = new List<string>() { "HotUpdate.dll" };


        private async UniTask LoadDlls()
        {
            await CacheAssembliesBytes();
            Log.Info("[LoadDlls] CacheAssemblies Finish");
            await LoadMetadataForAOTAssemblies();
            Log.Info("[LoadDlls] LoadMetadataForAOTAssemblies Finish");
            await LoadHotUpdateAssembly();
            Log.Info("[LoadDlls] LoadHotUpdateAssembly Finish");
            s_assetDatas.Clear();
        }

        private async UniTask CacheAssembliesBytes()
        {
            var totalFileNames = HotUpdateAssemblyFiles.Concat(AOTMetaAssemblyFiles);
            var tasks = new List<UniTask>();
            foreach (var fileName in totalFileNames)
            {
                tasks.Add(LoadAssemblyBytes(fileName));
            }
            await UniTask.WhenAll(tasks);
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

            var bytes = await Entrance.Resource.LoadRawFileAsync(location);
            s_assetDatas.Add(fileName, bytes);
            Log.Info($"Load {fileName}.bytes success");
            Log.Info("------------------------------------------------------------------");
        }

        private async UniTask LoadMetadataForAOTAssemblies()
        {
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            for (int i = 0; i < AOTMetaAssemblyFiles.Count; i++)
            {
                var bytes = ReadBytesFromCache(AOTMetaAssemblyFiles[i]);
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(bytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{AOTMetaAssemblyFiles[i]}. mode:{mode} ret:{err}");
            }
        }

        private async UniTask LoadHotUpdateAssembly()
        {
            Assembly assembly;
#if !UNITY_EDITOR
            byte[] assemblyData = ReadBytesFromCache("HotUpdate.dll");
            assembly = Assembly.Load(assemblyData);
            Log.Info($"Load assembly: {assembly.GetName()} success ");
#else
            assembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
            Log.Info($"Find assembly: {assembly.GetName()} success ");
#endif
        }
    }
}
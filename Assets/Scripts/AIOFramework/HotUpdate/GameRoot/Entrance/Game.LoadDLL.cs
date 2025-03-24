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
        private Dictionary<string, byte[]> _assetDatas = new Dictionary<string, byte[]>();
        private List<HandleBase> _handles = new List<HandleBase>();
        /// <summary>
        /// 来自AOTGenericReferences. 与HybridCLR.Setting面板中的Path AOT Assemblies保持一致
        /// </summary>
        private List<string> _aotMetaAssemblyFiles { get; } = new List<string>()
        {
            "mscorlib.dll.bytes",
            "System.dll.bytes",
            "System.Core.dll.bytes",
            "UnityEngine.CoreModule.dll.bytes",
            "AIOFramework.Runtime.dll.bytes",
            "Loxodon.Framework.dll.bytes",
            "UniTask.dll.bytes",
        };

        private async UniTask LoadDlls()
        {
            await CacheAssembliesBytes();
            Log.Info("[LoadDlls] CacheAssemblies Finish");
            LoadMetadataForAOTAssemblies();
            Log.Info("[LoadDlls] LoadMetadataForAOTAssemblies Finish");
            UnloadDllBundles();
            _assetDatas.Clear();
        }

        private async UniTask CacheAssembliesBytes()
        {
            var totalFileNames = _aotMetaAssemblyFiles;
            foreach (var fileName in totalFileNames)
            {
                await LoadAssemblyBytes(fileName);
            }
        }
        
        private byte[] ReadBytesFromCache(string dllName)
        {
            return _assetDatas[dllName];
        }
        
        private async UniTask LoadAssemblyBytes(string fileName)
        {
            var dllDirectory = Path.Combine("Assets",
                SettingUtility.GlobalSettings.GameSetting.HotUpdateDllDirectory);
            var location = Utility.Path.GetRegularPath(Path.Combine(dllDirectory, fileName));

            var dllText = await Entrance.Resource.LoadAssetAsync<TextAsset>(location);
            Log.Info($"load {location}");
            _assetDatas.Add(fileName, dllText.Item1.bytes);
            _handles.Add(dllText.Item2);
            Log.Info($"Load {fileName}.bytes success");
            Log.Info("------------------------------------------------------------------");
        }

        private void LoadMetadataForAOTAssemblies()
        {
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            for (int i = 0; i < _aotMetaAssemblyFiles.Count; i++)
            {
                var bytes = ReadBytesFromCache(_aotMetaAssemblyFiles[i]);
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(bytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{_aotMetaAssemblyFiles[i]}. mode:{mode} ret:{err}");
            }
        }

        private void UnloadDllBundles()
        {
            foreach (var handle in _handles)
            {
                Entrance.Resource.UnloadAsset(handle);
            }
            _handles.Clear();
        }
    }
}
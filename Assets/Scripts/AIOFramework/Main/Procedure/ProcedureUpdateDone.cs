using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AIOFramework.Procedure;
using AIOFramework.Setting;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    public class ProcedureUpdateDone : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Update Done");
            Entrance.Event.Fire(this, HotUpdateFinishEventArgs.Create());
            EnterGame().Forget();
        }
        
        private async UniTask EnterGame()
        {
            await LoadHotUpdateAssembly();
            
            await Entrance.Resource.InstantiateAsync<GameObject>("Assets/ArtAssets/HotUpdate/Game.prefab");
        }

        private async UniTask LoadHotUpdateAssembly()
        {
            Assembly assembly;
#if !UNITY_EDITOR
            var dllDirectory = Path.Combine("Assets",
                SettingUtility.GlobalSettings.GameSetting.HotUpdateDllDirectory);
            var location = Utility.Path.GetRegularPath(Path.Combine(dllDirectory, "HotUpdate.dll.bytes"));
            var result = await Entrance.Resource.LoadAssetAsync<TextAsset>(location);
            assembly = Assembly.Load(result.Item1.bytes);
            Entrance.Resource.UnloadAsset(result.Item2);
            Log.Info($"Load assembly: {assembly.GetName()} success ");
#else
            assembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
            Log.Info($"Find assembly: {assembly.GetName()} success ");
#endif
        }
    }
}
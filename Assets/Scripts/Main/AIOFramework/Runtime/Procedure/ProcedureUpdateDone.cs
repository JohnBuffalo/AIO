using System.Collections.Generic;
using System.Linq;
using AIOFramework.Procedure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    public class ProcedureUpdateDone : ProcedureBase
    {
        private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();
        private static List<string> HotUpdateAssemblyFiles { get; } = new List<string>()
        {
            "HotUpdate",
        };
        
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Update Done");
            Entrance.Event.Fire(this, HotUpdateFinishEventArgs.Create());
            EnterGame().Forget();
        }
        
        private async UniTask EnterGame()
        {
            await Entrance.Resource.InstantiateAsync<GameObject>("Assets/ArtAssets/HotUpdate/Game.prefab");
        }
    }
}
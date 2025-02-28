using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 启动流程入口
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Enter ProcedureLaunch");
            InitSettings();
            ChangeState<ProcedureSplash>(procedureOwner);

        }

        private void InitSettings()
        {
            
        }
    }
}
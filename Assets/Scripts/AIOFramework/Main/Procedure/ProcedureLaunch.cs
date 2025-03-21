using Cysharp.Threading.Tasks;
using AIOFramework.Procedure;
using UnityEngine;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 启动流程入口
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
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
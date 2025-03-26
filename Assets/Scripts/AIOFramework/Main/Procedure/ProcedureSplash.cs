using AIOFramework.Procedure;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 公司Logo,免责声明的展示等
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        private bool _splashFinished = false;

        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Enter ProcedureSplash");
            Splash();
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds,
            float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!_splashFinished) return;

            ChangeState<ProcedureInitPackage>(procedureOwner);
        }

        private void Splash()
        {
            _splashFinished = true;
        }
    }
}
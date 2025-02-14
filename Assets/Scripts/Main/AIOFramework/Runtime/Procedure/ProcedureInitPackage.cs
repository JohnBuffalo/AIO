using AIOFramework.Runtime.Setting;
using GameFramework.Procedure;
using YooAsset;
using Cysharp.Threading.Tasks;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using AIOFramework.Runtime.Setting;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 初始化ResourcePackage
    /// </summary>
    public class ProcedureInitPackage : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            InitPackage(procedureOwner).Forget();
        }

        private async UniTaskVoid InitPackage(ProcedureOwner procedureOwner)
        {
            var playMode = Entrance.Resource.PlayMode;
            var packageName = Entrance.Resource.PackageName;
        
            procedureOwner.SetData<VarInt32>("PlayMode", (int)playMode);
            procedureOwner.SetData<VarString>("PackageName", packageName);
        
            Log.Info($"InitPackage , playMode : {playMode}, packageName : {packageName}");

            var initSuccess = await Entrance.Resource.InitPackageAsync(packageName, GetHostServerURL(), GetDefaultServerURL(), true);

            if (initSuccess)
            {
                // ChangeState<ProcedureUpdatePackageVersion>(procedureOwner);
            }
            else
            {
                
            }
        }
        
        private string GetHostServerURL()
        {
            var serverType = SettingUtility.GlobalSettings.GameSetting.ServerType;
            string url;
            switch (serverType)
            {
                case ServerTypeEnum.Intranet:
                    url = SettingUtility.GlobalSettings.GameSetting.InnerResourceSourceUrl;
                    break;
                case ServerTypeEnum.Extranet:
                    url = SettingUtility.GlobalSettings.GameSetting.ExtraResourceSourceUrl;
                    break;
                case ServerTypeEnum.Formal:
                    url = SettingUtility.GlobalSettings.GameSetting.FormalResourceSourceUrl;
                    break;
                default:
                    url = string.Empty;
                    break;
            }
            return url;
        }

        private string GetDefaultServerURL()
        {
            return "http://127.0.0.1";
        }
    }

}
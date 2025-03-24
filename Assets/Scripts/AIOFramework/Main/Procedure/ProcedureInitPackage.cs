using System.Net;
using System.Security.Cryptography.X509Certificates;
using AIOFramework.Setting;
using AIOFramework.Procedure;
using AIOFramework.UI;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    /// <summary>
    /// 初始化ResourcePackage
    /// </summary>
    public partial class ProcedureInitPackage : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Enter ProcedureInitPackage");
            Entrance.Event.Fire(this, PatchStateChangeEventArgs.Create("InitPackage"));
            
            SetWebRequestDelegate();
            InitBindingService();
            InitPackage(procedureOwner).Forget();
        }

        private async UniTask InitPackage(ProcedureOwner procedureOwner)
        {
            var playMode = Entrance.Resource.PlayMode;
            var packageName = Entrance.Resource.PackageName;

            procedureOwner.SetData<VarInt32>("PlayMode", (int)playMode);
            procedureOwner.SetData<VarString>("PackageName", packageName);

            Log.Info($"InitPackage , playMode : {playMode}, packageName : {packageName}");

            await OpenPatchPage();
            
            var initSuccess =
                await Entrance.Resource.InitPackageAsync(packageName, GetHostServerURL(), GetDefaultServerURL(), true);

            if (initSuccess)
            {
                ChangeState<ProcedureUpdatePackageVersion>(procedureOwner);
            }
            else
            {
                Entrance.Event.Fire(this, InitPackageFailedEventArgs.Create());
            }
        }

        private string GetHostServerURL()
        {
            var serverType = SettingUtility.GlobalSettings.GameSetting.ServerType;
            string url;
            string platform = SettingUtility.PlatformName();
            switch (serverType)
            {
                case ServerTypeEnum.Local:
                    url = SettingUtility.GlobalSettings.GameSetting.LocalResourceUrl;
                    break;
                case ServerTypeEnum.Intranet:
                    url = SettingUtility.GlobalSettings.GameSetting.InnerResourceUrl;
                    break;
                case ServerTypeEnum.Extranet:
                    url = SettingUtility.GlobalSettings.GameSetting.ExtraResourceUrl;
                    break;
                case ServerTypeEnum.Formal:
                    url = SettingUtility.GlobalSettings.GameSetting.FormalResourceUrl;
                    break;
                default:
                    url = string.Empty;
                    break;
            }
            
            Log.Info($"GetHostServerURL, platform : {platform}, url : {url}");
            return $"{url}/{platform}/";
        }

        private string GetDefaultServerURL()
        {
            string platform = SettingUtility.PlatformName();
            string url = "http://127.0.0.1:8080";
            Log.Info($"GetDefaultServerURL, platform : {platform}, url : {url}");
            return $"{url}/{platform}/";
        }

        /// <summary>
        /// 在任意界面打开前,初始化MVVM的BindingService
        /// </summary>
        private void InitBindingService()
        {
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();
        }
        
        private async UniTask OpenPatchPage()
        {
            Log.Info("OpenPatchPage");
            ResourceRequest request = Resources.LoadAsync<GameObject>("Asset/PatchPage");
            var prefab = await request.ToUniTask();

            if (prefab == null)
            {
                Log.Error("Load PatchPage Failed");
                return;
            }

            GameObject patchPage = Object.Instantiate(prefab,UIRoot.Instance.Canvas.transform) as GameObject;
            PatchPage patchView = patchPage.GetComponent<PatchPage>();
            PatchViewModel patchViewModel = ReferencePool.Acquire<PatchViewModel>();
            patchViewModel.Model = new PatchModel();
            patchView.BindContext(patchViewModel);
        }
        
        
    }
    

    
}
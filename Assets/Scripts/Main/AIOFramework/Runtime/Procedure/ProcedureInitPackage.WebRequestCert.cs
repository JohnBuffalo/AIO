using AIOFramework.Setting;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using YooAsset;

namespace AIOFramework.Runtime
{
    public partial class ProcedureInitPackage
    {
        private void SetWebRequestDelegate()
        {
            var serverType = SettingUtility.GlobalSettings.GameSetting.ServerType;
            if (serverType != ServerTypeEnum.Local) return;
            
            YooAssets.SetDownloadSystemUnityWebRequest((url) =>
            {
                var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
                request.certificateHandler = new WebRequestCertificate();
                return request; 
            });
        }
        
        private async UniTask TestConnection(string server)
        {
            string url = $"{server}/DefaultPackage.version";
            var request = UnityEngine.Networking.UnityWebRequest.Get(url);
            request.certificateHandler = new WebRequestCertificate();
            Log.Info($"TestConnection {url}");
            await request.SendWebRequest();
    
            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Log.Info($"Error: {request.error}");
                Log.Info($"Response Code: {request.responseCode}");
                Log.Info($"Detailed Error: {request.downloadHandler.text}");
            }
            else
            {
                Log.Info("Success: " + request.downloadHandler.text);
            }
        }
    }
    
    public class WebRequestCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
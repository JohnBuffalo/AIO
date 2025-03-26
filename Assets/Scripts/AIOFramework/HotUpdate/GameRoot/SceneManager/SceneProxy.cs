using AIOFramework.UI;
using UnityEngine;
using YooAsset;
using UnityEngine.Rendering.Universal;

namespace AIOFramework.Runtime
{
    public class SceneProxy : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera = null;
        public Camera Camera => _camera;

        private SceneHandle _sceneHandle = null;
        public SceneHandle SceneHandle{
            get{
                return _sceneHandle;
            }
            set{
                _sceneHandle = value;
                
            }
        }

        public string SceneName{
            get{
                return _sceneHandle.SceneName;
            }
        }

        public void OnFocus()
        {
            Log.Info($"[SceneProxy] OnFocus {SceneName}");
            _camera.transform.tag = "MainCamera";   
            _camera.gameObject.SetActive(true);
            SetUICameraAsOverlay();
        }
        
        public void OnLooseFocus()
        {
            Log.Info($"[SceneProxy] OnLooseFocus {SceneName}");
            _camera.transform.tag = "Untagged";
            _camera.gameObject.SetActive(false);
            ClearCameraStack();
        }

        private void ClearCameraStack(){
            var cameraData = _camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Clear();
        }

        public void SetUICameraAsOverlay()
        {
            var uiCamera = UIRoot.Instance.Camera;
            var uiCameraData = uiCamera.GetUniversalAdditionalCameraData();
            uiCameraData.renderType = CameraRenderType.Overlay;
            var baseCameraData = _camera.GetUniversalAdditionalCameraData();
            baseCameraData.cameraStack.Add(uiCamera);
        }

        private void Awake()
        {
            var audioListener = _camera.GetComponent<AudioListener>();
            if(audioListener != null){
                audioListener.enabled = false;
            }
            _camera.gameObject.SetActive(false);
        }

        public override string ToString()
        {
            return $"SceneProxy: {SceneName} , valid : {_sceneHandle.IsValid}";
        }
    }
}
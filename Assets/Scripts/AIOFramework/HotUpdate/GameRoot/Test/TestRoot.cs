using Cysharp.Threading.Tasks;
using HotUpdate;
using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class TestRoot
    {
        private static TestRoot _instance;
        public static TestRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TestRoot();
                }
                return _instance;
            }
        }

        public async UniTask Test()
        {
            AddListeners();
            await TestUI();
            await TestScene();
        }

        public async UniTask TestUI()
        {
            // var ctorInfo = ReferencePool.Acquire<TestPageCtorInfo>();
            // ctorInfo.Tips = "1";
            // var uid = await Game.UI.OpenUI<TestPageViewModel>(ctorInfo);
            // await UniTask.Delay(1000);

            // var ctorInfo2 = ReferencePool.Acquire<TestPageCtorInfo2>();
            // ctorInfo2.Tips = "2";
            // await Game.UI.OpenUI<TestPageViewModel>(ctorInfo2);
            // await UniTask.Delay(1000);

            // ctorInfo = ReferencePool.Acquire<TestPageCtorInfo>();
            // ctorInfo.Tips = "1.1";
            // uid = await Game.UI.OpenUI<TestPageViewModel>(ctorInfo);
            // var uiPage = Game.UI.GetUI<TestPage>(uid);
            // await uiPage.GetViewModel<TestPageViewModel>().LoadImage();
            // await UniTask.Delay(1000);

            for (int i = 0; i < 3; i++)
            {
                var windowCtor = ReferencePool.Acquire<TestWindowCtorInfo>();
                var uid = await Game.UI.OpenUI<TestWindowViewModel>(windowCtor);
                var uiWindow = Game.UI.GetUI<TestWindow>(uid);
                uiWindow.GetViewModel<TestWindowViewModel>().Tips = uid.ToString();
            }

            // await UniTask.Delay(2000);
            // var toCloseUI = Game.UI.GetUI<TestPage>(ctorInfo.AssetName);
            // Game.UI.CloseUI(toCloseUI.SerialId);
        }

        public async UniTask TestScene()
        {

            //异步 加载-加载
            SceneProxy proxy = await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level1.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // SceneProxy proxy2 = await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level1.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // await Game.Scene.UnloadSceneAsync(proxy.SceneHandle, null);

            //同步 加载-加载
            // Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level1.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level1.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);

            //加载-卸载
            // SceneProxy proxy3 = await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // await Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");

            //同步 加载-卸载
            // Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");

            //卸载-加载
            await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // await Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");
            // await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);

            //同步 卸载-加载
            // Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");
            // Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);

            //卸载-卸载
            // await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // await Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");
            // await Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");

            //同步 卸载-卸载
            // await Game.Scene.LoadSceneAsync("Assets/ArtAssets/Scene/level2.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            // Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");
            // Game.Scene.UnloadSceneAsync("Assets/ArtAssets/Scene/level2.unity");

        }
    
        private void AddListeners(){
            Entrance.Event.Subscribe(ActiveSceneChangedEventArgs.s_EventId, (sender, args)=>{
                var e = args as ActiveSceneChangedEventArgs;
                Log.Info($"Active Scene Changed old : {e.LastActiveScene?.ToString()}, new : { e.ActiveScene?.ToString()}");
            });
        }
    }
}
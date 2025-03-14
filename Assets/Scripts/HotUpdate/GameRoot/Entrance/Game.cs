using AIOFramework.Runtime;
using AIOFramework.Resource;
using AIOFramework.Event;
using AIOFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HotUpdate
{
    public partial class Game : MonoBehaviour
    {
        public static BaseComponent Base { get; private set; }
        public static ResourceComponent Resource { get; private set; }
        public static EventComponent Event { get; private set; }
        public static UIComponent UI { get; private set; }


        private async void Start()
        {
            await LoadDlls();

            ReferToBuiltInComponents();
            InitHotUpdateComponents();
            DontDestroyOnLoad(this);
            
            Test().Forget();
        }
        
        private void InitHotUpdateComponents()
        {
             UI = GameEntry.GetComponent<UIComponent>();
             UI.Init(typeof(UIManager), UIRoot.Instance.Canvas.transform);
        }

        private void ReferToBuiltInComponents()
        {
            Base = Entrance.Base;
            Resource = Entrance.Resource;
            Event = Entrance.Event;
        }
        
        private async UniTask Test()
        {
            var uid = await Game.UI.OpenUI<TestPage2CtorInfo, TestPageViewModel>();
            var uiPage = Game.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "1";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            
            uid = await Game.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            uiPage = Game.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "2";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            Game.UI.CloseUI(uid);
            
            var ctorInfo = ReferencePool.Acquire<TestPage2CtorInfo>();
            var viewModel = ReferencePool.Acquire<TestPageViewModel>();
            var uid2 = await Game.UI.OpenUI(ctorInfo, viewModel, null);
            uiPage = Game.UI.GetUI<TestPage>(uid2);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "3";
            // ((GameObject)(uiPage.Handle)).name = uid2.ToString();
            // Log.Error($"open ui {uid2}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            
            uid = await Game.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            uiPage = Game.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "4";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);


        }

    }
}
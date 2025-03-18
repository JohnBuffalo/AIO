using AIOFramework.Runtime;
using AIOFramework.Resource;
using AIOFramework.Event;
using AIOFramework.Fsm;
using AIOFramework.ObjectPool;
using AIOFramework.Procedure;
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
        public static FsmComponent Fsm { get; private set; }
        public static ObjectPoolComponent ObjectPool { get; private set; }

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
            
            Fsm = GameEntry.GetComponent<FsmComponent>();
            Fsm.DestroyFsm<IProcedureManager>(); //删除热更状态机
            
            ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();
        }

        private void ReferToBuiltInComponents()
        {
            Base = Entrance.Base;
            Resource = Entrance.Resource;
            Event = Entrance.Event;
        }

        private async UniTask Test()
        {
            await TestUI();
            await TestFsm();
            await TestObjectPool();
        }

        private async UniTask TestUI()
        {
            var uid = await Game.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            var uiPage = Game.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = uid.ToString();
            await uiPage.GetViewModel<TestPageViewModel>().LoadImage();
            await UniTask.Delay(2000);
            Game.UI.CloseUI(uid);

            await UniTask.Delay(2000);
            uid = await Game.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            uiPage = Game.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "4";

        }

        private async UniTask TestFsm()
        {
        }

        private async UniTask TestObjectPool()
        {
            
        }
    }
}
using Cysharp.Threading.Tasks;
using HotUpdate;

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
            await TestUI();
        }
        
        public async UniTask TestUI()
        {
            var ctorInfo = ReferencePool.Acquire<TestPageCtorInfo>();
            ctorInfo.Tips = "1";
            var uid = await Game.UI.OpenUI<TestPageViewModel>(ctorInfo);
            await UniTask.Delay(1000);
            
            var ctorInfo2 = ReferencePool.Acquire<TestPageCtorInfo2>();
            ctorInfo2.Tips = "2";
            await Game.UI.OpenUI<TestPageViewModel>(ctorInfo2);
            await UniTask.Delay(1000);

            ctorInfo = ReferencePool.Acquire<TestPageCtorInfo>();
            ctorInfo.Tips = "1.1";
            uid = await Game.UI.OpenUI<TestPageViewModel>(ctorInfo);
            var uiPage = Game.UI.GetUI<TestPage>(uid);
            await uiPage.GetViewModel<TestPageViewModel>().LoadImage();
            await UniTask.Delay(1000);

            for (int i = 0; i < 3; i++)
            {
                var windowCtor = ReferencePool.Acquire<TestWindowCtorInfo>();
                uid = await Game.UI.OpenUI<TestWindowViewModel>(windowCtor);
                var uiWindow = Game.UI.GetUI<TestWindow>(uid);
                uiWindow.GetViewModel<TestWindowViewModel>().Tips = uid.ToString();
            }
            
            await UniTask.Delay(2000);
            var toCloseUI = Game.UI.GetUI<TestPage>(ctorInfo.AssetName);
            Game.UI.CloseUI(toCloseUI.SerialId);
        }
    }
}
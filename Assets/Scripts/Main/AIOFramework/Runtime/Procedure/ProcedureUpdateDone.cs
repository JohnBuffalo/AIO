using System.Collections.Generic;
using AIOFramework.Procedure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = AIOFramework.Fsm.IFsm<AIOFramework.Procedure.IProcedureManager>;

namespace AIOFramework.Runtime
{
    public class ProcedureUpdateDone : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Info("Update Done");
            Entrance.Event.Fire(this, HotUpdateFinishEventArgs.Create());

            Test().Forget();
        }

        private async UniTask Test()
        {
            var uid = await Entrance.UI.OpenUI<TestPage2CtorInfo, TestPageViewModel>();
            var uiPage = Entrance.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "1";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            
            uid = await Entrance.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            uiPage = Entrance.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "2";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            Entrance.UI.CloseUI(uid);
            
            var ctorInfo = ReferencePool.Acquire<TestPage2CtorInfo>();
            var viewModel = ReferencePool.Acquire<TestPageViewModel>();
            var uid2 = await Entrance.UI.OpenUI(ctorInfo, viewModel, null);
            uiPage = Entrance.UI.GetUI<TestPage>(uid2);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "3";
            // ((GameObject)(uiPage.Handle)).name = uid2.ToString();
            // Log.Error($"open ui {uid2}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);
            
            uid = await Entrance.UI.OpenUI<TestPageCtorInfo, TestPageViewModel>();
            uiPage = Entrance.UI.GetUI<TestPage>(uid);
            uiPage.GetViewModel<TestPageViewModel>().Tips = "4";
            // ((GameObject)(uiPage.Handle)).name = uid.ToString();
            // Log.Error($"open ui {uid}, {uiPage.GetComponent<Canvas>().sortingOrder}");
            await UniTask.Delay(2000);


        }
    }
}
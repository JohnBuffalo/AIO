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

            TestRoot.Instance.Test().Forget();
        }

        private void InitHotUpdateComponents()
        {
            ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();

            UI = GameEntry.GetComponent<UIComponent>();
            UI.Init(typeof(UIManager), UIRoot.Instance.Canvas.transform);
            
            Fsm = GameEntry.GetComponent<FsmComponent>();
            Fsm.DestroyFsm<IProcedureManager>(); //删除热更状态机
            
        }

        private void ReferToBuiltInComponents()
        {
            Base = Entrance.Base;
            Resource = Entrance.Resource;
            Event = Entrance.Event;
        }
        
    }
}
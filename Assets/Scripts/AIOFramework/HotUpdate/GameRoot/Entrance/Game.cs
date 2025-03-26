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
        public static SceneComponent Scene { get; private set; }
        private async void Start()
        {
            await LoadDlls();

            ReferToBuiltInComponents();
            InitHotUpdateComponents();
            Fsm.DestroyFsm<IProcedureManager>(); //删除热更状态机
            DontDestroyOnLoad(this);

            _ = TestRoot.Instance.Test();
        }

        private void InitHotUpdateComponents()
        {
            UI = GameEntry.GetComponent<UIComponent>();
            UI.Init(typeof(UIManager), UIRoot.Instance.Canvas.transform);
            Scene = GameEntry.GetComponent<SceneComponent>();
        }

        private void ReferToBuiltInComponents()
        {
            Base = Entrance.Base;
            Resource = Entrance.Resource;
            Event = Entrance.Event;
            Fsm = Entrance.Fsm;
            ObjectPool = Entrance.ObjectPool;
        }
        
    }
}
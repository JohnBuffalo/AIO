using UnityEngine;
using AIOFramework.Event;
using AIOFramework.Fsm;
using AIOFramework.ObjectPool;
using AIOFramework.Resource;

namespace AIOFramework.Runtime
{
    public class Entrance : MonoBehaviour
    {
        public static BaseComponent Base { get; private set; }
        public static ResourceComponent Resource { get; private set; }
        public static EventComponent Event { get; private set; }
        public static ProcedureComponent Procedure { get; private set; }
        public static FsmComponent Fsm { get; private set; }
        public static ObjectPoolComponent ObjectPool { get; private set; }
        public static ReferencePoolComponent ReferencePool { get; private set; }

        private void InitBuiltinComponents()
        {
            Base = GameEntry.GetComponent<BaseComponent>();
            Resource = GameEntry.GetComponent<ResourceComponent>();
            Event = GameEntry.GetComponent<EventComponent>();
            Procedure = GameEntry.GetComponent<ProcedureComponent>();
            Fsm = GameEntry.GetComponent<FsmComponent>();
            ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();
            ReferencePool = GameEntry.GetComponent<ReferencePoolComponent>();
        }

        private void Start()
        {
            InitBuiltinComponents();
            DontDestroyOnLoad(this);
        }
    }
}
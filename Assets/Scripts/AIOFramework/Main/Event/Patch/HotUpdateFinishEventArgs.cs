using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class HotUpdateFinishEventArgs : Event.BaseEventArgs
    {
        public static readonly int s_EventID = typeof(HotUpdateFinishEventArgs).GetHashCode();
        public override int Id => s_EventID;
        
        public static HotUpdateFinishEventArgs Create()
        {
            var args = ReferencePool.Acquire<HotUpdateFinishEventArgs>();
            return args;
        }
        public override void Clear()
        {
        }

    }
}
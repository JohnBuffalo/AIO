using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class HotUpdateFinishEventArgs : GameEventArgs
    {
        public static readonly int EventID = typeof(HotUpdateFinishEventArgs).GetHashCode();
        public override int Id => EventID;
        
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
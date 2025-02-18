using GameFramework.Event;
using GameFramework;

namespace AIOFramework.Runtime
{
    public class PatchStateChangeEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(PatchStateChangeEvent).GetHashCode();
        public override int Id => EventId;
        public string Tips { get; private set; }
        public override void Clear()
        {
            Tips = null;
        }

        public static PatchStateChangeEvent Create(string tips)
        {
            var args = ReferencePool.Acquire<PatchStateChangeEvent>();
            args.Tips = tips;
            return args;
        }
    }
}
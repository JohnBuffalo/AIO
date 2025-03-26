using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class PatchStateChangeEventArgs : Event.BaseEventArgs
    {
        public static readonly int s_EventId = typeof(PatchStateChangeEventArgs).GetHashCode();
        public override int Id => s_EventId;
        public string Tips { get; private set; }
        public override void Clear()
        {
            Tips = null;
        }

        public static PatchStateChangeEventArgs Create(string tips)
        {
            var args = ReferencePool.Acquire<PatchStateChangeEventArgs>();
            args.Tips = tips;
            return args;
        }
    }
}
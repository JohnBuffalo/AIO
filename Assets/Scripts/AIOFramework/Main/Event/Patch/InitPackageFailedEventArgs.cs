using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class InitPackageFailedEventArgs : Event.BaseEventArgs
    {
        public override void Clear()
        {
        }

        public static readonly int s_EventId = typeof(InitPackageFailedEventArgs).GetHashCode();
        public override int Id => s_EventId;

        public static InitPackageFailedEventArgs Create()
        {
            var args = ReferencePool.Acquire<InitPackageFailedEventArgs>();
            return args;
        }
    }
}
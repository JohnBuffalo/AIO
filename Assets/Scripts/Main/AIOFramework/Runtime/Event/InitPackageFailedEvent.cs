using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class InitPackageFailedEvent : GameEventArgs
    {
        public override void Clear()
        {
        }

        public static readonly int EventId = typeof(InitPackageFailedEvent).GetHashCode();
        public override int Id => EventId;

        public static InitPackageFailedEvent Create()
        {
            var args = ReferencePool.Acquire<InitPackageFailedEvent>();
            return args;
        }
    }
}
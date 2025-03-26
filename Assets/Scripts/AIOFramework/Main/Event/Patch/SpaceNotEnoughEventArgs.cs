using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class SpaceNotEnoughEventArgs: Event.BaseEventArgs
    {
        public static readonly int s_EventId = typeof(SpaceNotEnoughEventArgs).GetHashCode();
        public override int Id => s_EventId;
        
        public long NeedSpace { get; private set; }
        public long FreeSpace { get; private set; }

        public static SpaceNotEnoughEventArgs Create(long needSpace, long freeSpace)
        {
            var args = ReferencePool.Acquire<SpaceNotEnoughEventArgs>();
            args.NeedSpace = needSpace;
            args.FreeSpace = freeSpace;
            return args;
        }
        
        public override void Clear()
        {
            NeedSpace = 0;
            FreeSpace = 0;
        }

    }
}
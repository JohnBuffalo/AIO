using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class SpaceNotEnoughEventArgs:GameEventArgs
    {
        public static readonly int EventId = typeof(SpaceNotEnoughEventArgs).GetHashCode();
        public override int Id => EventId;
        
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
using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class PackageVersionEvent : GameEventArgs
    {
        public override void Clear()
        {
            PackageVersion = null;
        }
        public static readonly int EventId = typeof(PackageVersionEvent).GetHashCode();
        public override int Id => EventId;
        public string PackageVersion { get; private set; }
        public static PackageVersionEvent Create(string version)
        {
            var args = ReferencePool.Acquire<PackageVersionEvent>();
            args.PackageVersion = version;
            return args;
        }
    }
}
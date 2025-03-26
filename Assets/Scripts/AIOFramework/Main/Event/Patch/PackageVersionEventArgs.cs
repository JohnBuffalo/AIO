using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class PackageVersionEventArgs : Event.BaseEventArgs
    {
        public override void Clear()
        {
            PackageVersion = null;
        }
        public static readonly int s_EventId = typeof(PackageVersionEventArgs).GetHashCode();
        public override int Id => s_EventId;
        public string PackageVersion { get; private set; }
        public static PackageVersionEventArgs Create(string version)
        {
            var args = ReferencePool.Acquire<PackageVersionEventArgs>();
            args.PackageVersion = version;
            return args;
        }
    }
}
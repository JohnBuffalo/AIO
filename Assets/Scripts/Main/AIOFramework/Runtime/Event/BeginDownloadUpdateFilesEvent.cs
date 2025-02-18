using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class BeginDownloadUpdateFilesEvent : GameEventArgs
    {
        public override void Clear()
        {
        }

        public static readonly int EventId = typeof(BeginDownloadUpdateFilesEvent).GetHashCode();
        public override int Id => EventId;
        
        public static BeginDownloadUpdateFilesEvent Create()
        {
            BeginDownloadUpdateFilesEvent args = ReferencePool.Acquire<BeginDownloadUpdateFilesEvent>();
            return args;
        }
    }
}
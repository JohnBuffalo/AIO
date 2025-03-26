
using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class BeginDownloadUpdateFilesEventArgs : Event.BaseEventArgs
    {
        public override void Clear()
        {
        }

        public static readonly int s_EventId = typeof(BeginDownloadUpdateFilesEventArgs).GetHashCode();
        public override int Id => s_EventId;
        
        public static BeginDownloadUpdateFilesEventArgs Create()
        {
            BeginDownloadUpdateFilesEventArgs args = ReferencePool.Acquire<BeginDownloadUpdateFilesEventArgs>();
            return args;
        }
    }
}
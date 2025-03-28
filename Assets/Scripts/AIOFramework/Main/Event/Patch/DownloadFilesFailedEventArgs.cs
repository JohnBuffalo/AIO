using AIOFramework.Event;
using AIOFramework.Runtime;

namespace AIOFramework
{
    public class DownloadFilesFailedEventArgs : Event.BaseEventArgs
    {
        public static readonly int s_EventId = typeof(DownloadFilesFailedEventArgs).GetHashCode();
        public override int Id => s_EventId;

        public string FileName { get; private set; }
        public string Error { get; private set; }
        public string PackageName { get; private set; }
        
        public static DownloadFilesFailedEventArgs Create(string package, string fileName, string error)
        {
            var args = ReferencePool.Acquire<DownloadFilesFailedEventArgs>();
            args.FileName = fileName;
            args.Error = error;
            args.PackageName = package;
            return args;
        }
        
        public override void Clear()
        {
            FileName = null;
            Error = null;
        }
    }
}
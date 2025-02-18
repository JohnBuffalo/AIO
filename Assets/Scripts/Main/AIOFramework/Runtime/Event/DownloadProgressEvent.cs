using GameFramework;
using GameFramework.Event;

namespace AIOFramework.Runtime
{
    public class DownloadProgressEvent : GameEventArgs
    {
        public static readonly int EventID = typeof(DownloadProgressEvent).GetHashCode();
        public override int Id => EventID;

        public int TotalDownloadCount { get; private set; }
        public int CurrentDownloadCount { get; private set; }
        public long TotalDownloadSizeBytes { get; private set; }
        public long CurrentDownloadSizeBytes { get; private set; }

        public override void Clear()
        {
            TotalDownloadSizeBytes = 0;
            CurrentDownloadCount = 0;
            TotalDownloadCount = 0;
            CurrentDownloadSizeBytes = 0;
        }
        
        public static DownloadProgressEvent Create(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            var args = ReferencePool.Acquire<DownloadProgressEvent>();
            args.TotalDownloadCount = totalDownloadCount;
            args.CurrentDownloadCount = currentDownloadCount;
            args.TotalDownloadSizeBytes = totalDownloadSizeBytes;
            args.CurrentDownloadSizeBytes = currentDownloadSizeBytes;
            return args;
        }
    }
}
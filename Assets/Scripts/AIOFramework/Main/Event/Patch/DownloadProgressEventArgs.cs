using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class DownloadProgressEventArgs : Event.BaseEventArgs
    {
        public static readonly int s_EventID = typeof(DownloadProgressEventArgs).GetHashCode();
        public override int Id => s_EventID;

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
        
        public static DownloadProgressEventArgs Create(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            var args = ReferencePool.Acquire<DownloadProgressEventArgs>();
            args.TotalDownloadCount = totalDownloadCount;
            args.CurrentDownloadCount = currentDownloadCount;
            args.TotalDownloadSizeBytes = totalDownloadSizeBytes;
            args.CurrentDownloadSizeBytes = currentDownloadSizeBytes;
            return args;
        }
    }
}
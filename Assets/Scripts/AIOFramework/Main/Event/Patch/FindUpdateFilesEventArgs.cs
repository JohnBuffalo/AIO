using AIOFramework.Event;
using AIOFramework.Runtime;
using UnityEngine;

namespace AIOFramework
{
    public class FindUpdateFilesEventArgs : Event.BaseEventArgs
    {
        public static readonly int s_EventId = typeof(FindUpdateFilesEventArgs).GetHashCode();

        public int TotalCount { get; private set; }
        public long TotalSizeBytes { get; private set; }
        public override int Id => s_EventId;

        public static FindUpdateFilesEventArgs Create(int totalCount, long totalSizeBytes)
        {
            var findUpdateFiles = ReferencePool.Acquire<FindUpdateFilesEventArgs>();
            findUpdateFiles.TotalCount = totalCount;
            findUpdateFiles.TotalSizeBytes = totalSizeBytes;
            return findUpdateFiles;
        }
        
        public override void Clear()
        {
            TotalCount = 0;
            TotalSizeBytes = 0;
        }

    }
}
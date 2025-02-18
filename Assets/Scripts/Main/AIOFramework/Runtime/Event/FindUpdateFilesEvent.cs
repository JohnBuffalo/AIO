using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace AIOFramework
{
    public class FindUpdateFilesEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(FindUpdateFilesEvent).GetHashCode();

        public int TotalCount { get; private set; }
        public long TotalSizeBytes { get; private set; }
        public override int Id => EventId;

        public static FindUpdateFilesEvent Create(int totalCount, long totalSizeBytes)
        {
            var findUpdateFiles = ReferencePool.Acquire<FindUpdateFilesEvent>();
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
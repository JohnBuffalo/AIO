

using AIOFramework.Runtime;
using YooAsset;

namespace AIOFramework
{
    public class VarDownloader : Variable<ResourceDownloaderOperation>
    {
        public VarDownloader(){}
        
        public static implicit operator VarDownloader(ResourceDownloaderOperation value)
        {
            VarDownloader varValue = ReferencePool.Acquire<VarDownloader>();
            varValue.Value = value;
            return varValue;
        }
        
        public static implicit operator ResourceDownloaderOperation(VarDownloader value)
        {
            return value.Value;
        }
    }  
}



using System.Collections.Generic;
using AIOFramework.Resource;
using Loxodon.Framework.ViewModels;
using AIOFramework.Runtime;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace AIOFramework.UI
{
    public abstract class UIViewModelBase : ViewModelBase, IReference
    {
        public virtual void Clear()
        {
            Dispose();
        }
    }
}
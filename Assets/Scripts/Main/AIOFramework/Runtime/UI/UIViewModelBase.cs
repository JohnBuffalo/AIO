
using Loxodon.Framework.ViewModels;
using AIOFramework.Runtime;

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

using Loxodon.Framework.ViewModels;
namespace AIOFramework.Runtime
{
    public abstract class UIViewModelBase : ViewModelBase, IReference
    {
        public virtual void Clear()
        {
            Dispose();
        }
        
    }
}
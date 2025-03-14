using Loxodon.Framework.Commands;
using AIOFramework.UI;

namespace AIOFramework.Runtime
{
    public class MessageBoxViewModel : UIViewModelBase
    {
        private string title;
        private string tip;
        private bool display;
        private SimpleCommand okCommand;

        public SimpleCommand OkCommand
        {
            get { return this.okCommand; }
            set { this.Set(ref okCommand, value); }
        }

        public string Tip
        {
            get { return tip; }
            set { this.Set(ref tip, value); }
        }
        
        public bool Display
        {
            get { return display; }
            set { this.Set(ref display, value); }
        }
        
        public string Title
        {
            get { return title; }
            set { this.Set(ref title, value); }
        }
    }
}
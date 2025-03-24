using Loxodon.Framework.Commands;
using AIOFramework.UI;

namespace AIOFramework.Runtime
{
    public class MessageBoxViewModel : UIViewModelBase
    {
        private string _title;
        private string _tip;
        private bool _display;
        private SimpleCommand _okCommand;

        public SimpleCommand OkCommand
        {
            get { return this._okCommand; }
            set { this.Set(ref _okCommand, value); }
        }

        public string Tip
        {
            get { return _tip; }
            set { this.Set(ref _tip, value); }
        }
        
        public bool Display
        {
            get { return _display; }
            set { this.Set(ref _display, value); }
        }
        
        public string Title
        {
            get { return _title; }
            set { this.Set(ref _title, value); }
        }
    }
}
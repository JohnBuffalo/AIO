using AIOFramework.Runtime;
using AIOFramework.UI;

namespace AIOFramework.Event
{
    public class CloseUICompleteEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CloseUICompleteEventArgs).GetHashCode();
        public override int Id => EventId;

        public int SerialId
        {
            get;
            private set;
        } = 0;

        public string UIName
        {
            get;
            private set;
        } = null;

        public IUIGroup UIGroup
        {
            get;
            private set;
        } = null;

        public object UserData
        {
            get;
            private set;
        } = null;


        public override void Clear()
        {
            SerialId = 0;
            UIName = null;
            UIGroup = null;
            UserData = null;
        }

        public static CloseUICompleteEventArgs Create(int serialId, string uiName, IUIGroup uiGroup, object userData)
        {
            CloseUICompleteEventArgs args = ReferencePool.Acquire<CloseUICompleteEventArgs>();
            args.SerialId = serialId;
            args.UIName = uiName;
            args.UIGroup = uiGroup;
            args.UserData = userData;
            return args;
        }

    }
}
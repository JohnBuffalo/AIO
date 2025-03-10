using AIOFramework.Event;

namespace AIOFramework.Runtime
{
    public class OpenUIFailureEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OpenUIFailureEventArgs).GetHashCode();
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

        public string UIGroupName
        {
            get;
            private set;
        } = null;

        public string ErrorMessage
        {
            get;
            private set;
        } = null;

        public object UserData
        {
            get;
            private set;
        } = null;

        public static OpenUIFailureEventArgs Create(int serialId, string uiName, string uiGroupName,string errorMessage, object userData)
        {
            OpenUIFailureEventArgs args = ReferencePool.Acquire<OpenUIFailureEventArgs>();
            args.SerialId = serialId;
            args.UIName = uiName;
            args.UIGroupName = uiGroupName;
            args.ErrorMessage = errorMessage;
            args.UserData = userData;
            return args;
        }
        
        public override void Clear()
        {
            SerialId = 0;
            UIName = null;
            UIGroupName = null;
            ErrorMessage = null;
            UserData = null;
        }

    }
}
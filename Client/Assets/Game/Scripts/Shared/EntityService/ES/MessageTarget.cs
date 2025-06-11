namespace EntityService {
    public abstract class MessageTarget : ESObject, IMessageTarget {
        protected MessageTarget() { }
        protected MessageTarget(ESClass info) : base(info) { }
        public int Handle { get; private set; }
        public void SetHadnle(int handle)
        {
            Handle = handle;
        }

        public void SendMsg(IMessageTarget target, string msgId, MsgBody msgBody)
        {
            target?.HandleMsg(this, msgId, msgBody);
        }

        public void HandleMsg(IMessageTarget sender, string msgId, MsgBody msgBody)
        {
            OnReceiveMsg(sender, msgId, msgBody);
        }

        protected virtual void OnReceiveMsg(IMessageTarget sender, string msgId, MsgBody msgBody) { }

        public virtual void Dispose()
        {

        }
    }
}
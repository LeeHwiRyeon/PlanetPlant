using System;

namespace EntityService {
    public interface IMessageTarget : IHandled, IEntity, IPropertyTable, ICP, IDisposable {
        void SendMsg(IMessageTarget target, string msgId, MsgBody msg);
        void HandleMsg(IMessageTarget sender, string msgId, MsgBody msg);
    }
}
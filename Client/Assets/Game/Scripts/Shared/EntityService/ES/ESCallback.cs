using Cysharp.Text;
using System;

namespace EntityService {
    public class NotifyError {
        public string Invoker;
        public string Message;
    }

    public class NotifyInfo {
        public string Invoker;
        public string Message;
    }

    public static class ESCallback {
        public static event Action<NotifyError> OnError;
        public static event Action<NotifyInfo> OnInfo;

        internal static void Error(object invoker, string message)
        {
            OnError?.Invoke(new NotifyError {
                Invoker = ZString.Format("{0}", invoker),
                Message = message,
            }); ;
        }

        internal static void Error(object invoker, string message, params object[] args)
        {
            Error(invoker, string.Format(message, args));
        }

        internal static void Info(object invoker, string message)
        {
            OnInfo?.Invoke(new NotifyInfo {
                Invoker = ZString.Format("{0}", invoker),
                Message = message
            });
        }

        internal static void Info(object invoker, string messageFormat, params object[] args)
        {
            Info(invoker, string.Format(messageFormat, args));
        }
    }
}

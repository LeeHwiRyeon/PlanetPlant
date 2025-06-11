using Cysharp.Text;
using System.Collections.Generic;
using System.Text;

namespace GameLogger {
    public class UnityLogger : ILogger {
        public UnityLogger() { }

        public void Trace(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Debug(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Info(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Warn(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public void Error(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public void Fatal(string logHeader, string msg, string color)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public StringBuilder GetStringBuilderFormLogs()
        {
            return new StringBuilder();
        }

        public StringBuilder GetStringBuilderFormErrorLogs()
        {
            return new StringBuilder();
        }

        public void Clear()
        {

        }
    }

    public class DUnityLogger : ILogger {
        // TRACE > DEBUG > INFO > WARN > ERROR > FATAL
        // enabled of log level
        public static bool IsTraceEnabled { get; set; }
        public static bool IsDebugEnabled { get; set; }
        public static bool IsInfoEnabled { get; set; }
        public static bool IsWarnEnabled { get; set; }
        public static bool IsErrorEnabled { get; set; }
        public static bool IsFatalEnabled { get; set; }

        public static Dictionary<string, bool> LoggerSettings { get; private set; }
        public static List<string> Logs { get; private set; }
        public static List<string> ErrorLogs { get; private set; }

        /// <summary> log filter msg </summary>
        public static string LogFilterText { get; set; }
        public static string LogHeaderText { get; set; }

        public DUnityLogger()
        {
            IsTraceEnabled = true;
            IsDebugEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
            IsInfoEnabled = true;
            IsWarnEnabled = true;
            LoggerSettings = new Dictionary<string, bool>();
            Logs = new List<string>();
            ErrorLogs = new List<string>();
            LogFilterText = string.Empty;
        }

        public void Trace(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsTraceEnabled || !enabled)
                return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Trace, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Trace, logHeader, msg);
                }
                UnityEngine.Debug.Log(msg);

                Logs.Add(msg);
            }
        }

        public void Debug(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsDebugEnabled || !enabled)
                return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Debug, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Debug, logHeader, msg);
                }
                UnityEngine.Debug.Log(msg);

                Logs.Add(msg);
            }
        }

        public void Info(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsInfoEnabled || !enabled)
                return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Info, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Info, logHeader, msg);
                }
                UnityEngine.Debug.Log(msg);

                Logs.Add(msg);
            }
        }

        public void Warn(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsWarnEnabled || !enabled) return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Warn, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Warn, logHeader, msg);
                }
                UnityEngine.Debug.LogWarning(msg);

                Logs.Add(msg);
            }
        }

        public void Error(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsErrorEnabled || !enabled)
                return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Error, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Error, logHeader, msg);
                }
                UnityEngine.Debug.LogError(msg);

                ErrorLogs.Add(msg);
            }
        }

        public void Fatal(string logHeader, string msg, string color)
        {
            if (!LoggerSettings.TryGetValue(logHeader, out var enabled)) {
                LoggerSettings[logHeader] = true;
                enabled = true;
            }

            if (!IsFatalEnabled || !enabled)
                return;

            if (string.IsNullOrEmpty(LogFilterText) || msg.Contains(LogFilterText)) {
                if (!string.IsNullOrEmpty(color)) {
                    msg = ZString.Format("[{0}]::[{1}]<color={2}>{3}</color>", ELogLevel.Fatal, logHeader, color, msg);
                } else {
                    msg = ZString.Format("[{0}]::[{1}]::{2}", ELogLevel.Fatal, logHeader, msg);
                }
                UnityEngine.Debug.LogError(msg);

                Logs.Add(msg);
            }
        }

        public StringBuilder GetStringBuilderFormLogs()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Logs.Count; i++) {
                var log = Logs[i];
                sb.AppendLine(log);
            }

            return sb;
        }


        public StringBuilder GetStringBuilderFormErrorLogs()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < ErrorLogs.Count; i++) {
                var log = ErrorLogs[i];
                sb.AppendLine(log);
            }

            return sb;
        }

        public void Clear()
        {
            Logs.Clear();
            ErrorLogs.Clear();
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameLogger {
    public static class LogColor {
        public static string Aqua = "#00ffffff";
        public static string Black = "#000000ff";
        public static string Blue = "#0000ffff";
        public static string Brown = "#a52a2aff";
        public static string Cyan = "#00ffffff";
        public static string Darkblue = "#0000a0ff";
        public static string Fuchsia = "#ff00ffff";
        public static string Green = "#008000ff";
        public static string Grey = "#808080ff";
        public static string Lightblue = "#add8e6ff";
        public static string Lime = "#00ff00ff";
        public static string Magenta = "#ff00ffff";
        public static string Maroon = "#800000ff";
        public static string Navy = "#000080ff";
        public static string Olive = "#808000ff";
        public static string Orange = "#ffa500ff";
        public static string Purple = "#800080ff";
        public static string Red = "#ff0000ff";
        public static string Silver = "#c0c0c0ff";
        public static string Teal = "#008080ff";
        public static string White = "#ffffffff";
        public static string Yellow = "#ffff00ff";
    }

    public enum ELogLevel {
        /// <summary> 추적 레벨은 Debug보다 좀더 상세한 정보를 나타냄 </summary>
        Trace,
        /// <summary> 프로그램을 디버깅하기 위한 정보 지정  </summary>
        Debug,
        /// <summary> 상태변경과 같은 정보성 메시지를 나타냄 </summary>
        Info,
        /// <summary> 처리 가능한 문제, 향후 시스템 에러의 원인이 될 수 있는 경고성 메시지를 나타냄 </summary>
        Warn,
        /// <summary> ERROR : 요청을 처리하는 중 문제가 발생한 경우 </summary>
        Error,
        /// <summary> 아주 심각한 에러가 발생한 상태, 시스템적으로 심각한 문제가 발생해서 어플리케이션 작동이 불가능할 경우 </summary>
        Fatal,
    }

    public interface ILogger {
        void Trace(string logHeader, string msg, string color);
        void Debug(string logHeader, string msg, string color);
        void Info(string logHeader, string msg, string color);
        void Warn(string logHeader, string msg, string color);
        void Error(string logHeader, string msg, string color);
        void Fatal(string logHeader, string msg, string color);
        public StringBuilder GetStringBuilderFormLogs();
        public StringBuilder GetStringBuilderFormErrorLogs();
        public void Clear();

    }

    public static class Log {
        private const string SYMBOL_NAME = "ENABLE_LOG";

        private static ILogger _logger = new DUnityLogger();

        public static void Init(ILogger logger)
        {
            _logger = logger;
        }

        public static bool IsActive()
        {
            return (_logger != null);
        }

        [Conditional(SYMBOL_NAME)]
        public static void Trace(string logHeader, string msg, string color = "")
        {
            _logger.Trace(logHeader, msg, color);
        }

        [Conditional(SYMBOL_NAME)]
        public static void Debug(string logHeader, string msg, string color = "")
        {
            _logger.Debug(logHeader, msg, color);
        }

        [Conditional(SYMBOL_NAME)]
        public static void Info(string logHeader, string msg, string color = "")
        {
            _logger.Info(logHeader, msg, color);
        }

        [Conditional(SYMBOL_NAME)]
        public static void Warn(string logHeader, string msg, string color = "")
        {
            _logger.Error(logHeader, msg, color);
        }

        [Conditional(SYMBOL_NAME)]
        public static void Error(string logHeader, string msg, string color = "")
        {
            _logger.Error(logHeader, msg, color);
        }
        
        [Conditional(SYMBOL_NAME)]
        public static void Fatal(string logHeader, string msg, string color = "")
        {
            _logger.Fatal(logHeader, msg, color);
        }

        public static StringBuilder GetStringBuilderFormLogs()
        {
            return _logger.GetStringBuilderFormLogs();
        }
        public static StringBuilder GetStringBuilderFormErrorLogs()
        {
            return _logger.GetStringBuilderFormErrorLogs();
        }

        public static void Clear()
        {
            _logger.Clear();
        }
    }
}
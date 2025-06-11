#if UNITY_EDITOR
using GameLogger;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GameLoggerWindow : EditorWindow {
    [MenuItem("Window/GameLogger")]
    private static void Init()
    {
        var window = GetWindow<GameLoggerWindow>(false, "GameLogger 설정");
        window.Show();
    }

    UnityLogger print = new UnityLogger();
    private void OnGUI()
    {
        EditorGUILayout.LabelField("로그 레벨 On/Off 설정");
        DUnityLogger.IsTraceEnabled = EditorGUILayout.Toggle("Trace", DUnityLogger.IsTraceEnabled);
        DUnityLogger.IsDebugEnabled = EditorGUILayout.Toggle("Debug", DUnityLogger.IsDebugEnabled);
        DUnityLogger.IsInfoEnabled = EditorGUILayout.Toggle("Info", DUnityLogger.IsInfoEnabled);
        DUnityLogger.IsWarnEnabled = EditorGUILayout.Toggle("Warn", DUnityLogger.IsWarnEnabled);
        DUnityLogger.IsErrorEnabled = EditorGUILayout.Toggle("Error", DUnityLogger.IsErrorEnabled);
        DUnityLogger.IsFatalEnabled = EditorGUILayout.Toggle("Fatal", DUnityLogger.IsFatalEnabled);

        // logHeader
        if (DUnityLogger.LoggerSettings != null) {
            EditorGUILayout.LabelField("---------------------------");
            EditorGUILayout.LabelField("특정 로그 On/Off 설정");
            foreach (var name in DUnityLogger.LoggerSettings.Keys.ToArray()) {
                var enabled = DUnityLogger.LoggerSettings[name];
                DUnityLogger.LoggerSettings[name] = EditorGUILayout.Toggle(name, enabled);
            }
        }

        // filter
        if (DUnityLogger.LogFilterText != null) {
            EditorGUILayout.LabelField("---------------------------");

            EditorGUILayout.LabelField("로그 메시지 필터 설정");
            bool rePrint = false;
            if (GUILayout.Button("로그 메시지 다시출력")) {
                rePrint = true;
            }
            DUnityLogger.LogFilterText = EditorGUILayout.TextField(DUnityLogger.LogFilterText);

            if (rePrint) {
                var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);

                var temps = DUnityLogger.Logs.ToArray();
                var trace = ELogLevel.Trace.ToString();
                var debug = ELogLevel.Debug.ToString();
                var info = ELogLevel.Info.ToString();
                var warn = ELogLevel.Warn.ToString();
                var error = ELogLevel.Error.ToString();
                var fatal = ELogLevel.Fatal.ToString();
                foreach (var temp in temps) {
                    if (string.IsNullOrEmpty(DUnityLogger.LogFilterText) == false
                        && temp.Contains(DUnityLogger.LogFilterText)) {
                        continue;
                    }

                    if (temp.Contains(trace)) {
                        print.Trace("", temp, "");
                    } else if (temp.Contains(debug)) {
                        print.Debug("", temp, "");
                    } else if (temp.Contains(info)) {
                        print.Info("", temp, "");
                    } else if (temp.Contains(warn)) {
                        print.Warn("", temp, "");
                    } else if (temp.Contains(error)) {
                        print.Error("", temp, "");
                    } else if (temp.Contains(fatal)) {
                        print.Fatal("", temp, "");
                    }
                }
            }


        }


    }
}
#endif
using GameLogger;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Collects logs during runtime tests so they can be inspected after execution.
/// Allows easy addition or removal of log entries for balancing analysis.
/// </summary>
public class TestGameLogger : ILogger
{
    public readonly List<string> Logs = new List<string>();
    public readonly List<string> ErrorLogs = new List<string>();

    private void Add(List<string> target, string header, string msg, string color)
    {
        var entry = string.IsNullOrEmpty(color)
            ? $"[{header}] {msg}"
            : $"[{header}] <color={color}>{msg}</color>";
        target.Add(entry);
        UnityEngine.Debug.Log(entry);
    }

    public void Trace(string logHeader, string msg, string color) => Add(Logs, logHeader, msg, color);
    public void Debug(string logHeader, string msg, string color) => Add(Logs, logHeader, msg, color);
    public void Info(string logHeader, string msg, string color) => Add(Logs, logHeader, msg, color);
    public void Warn(string logHeader, string msg, string color) => Add(Logs, logHeader, msg, color);
    public void Error(string logHeader, string msg, string color) => Add(ErrorLogs, logHeader, msg, color);
    public void Fatal(string logHeader, string msg, string color) => Add(ErrorLogs, logHeader, msg, color);

    public StringBuilder GetStringBuilderFormLogs()
    {
        var sb = new StringBuilder();
        foreach (var l in Logs)
        {
            sb.AppendLine(l);
        }
        return sb;
    }

    public StringBuilder GetStringBuilderFormErrorLogs()
    {
        var sb = new StringBuilder();
        foreach (var l in ErrorLogs)
        {
            sb.AppendLine(l);
        }
        return sb;
    }

    public void Clear()
    {
        Logs.Clear();
        ErrorLogs.Clear();
    }

    /// <summary>
    /// Save collected logs to a file under the supplied path.
    /// This is used instead of uploading to any external service
    /// so results remain local for analysis.
    /// </summary>
    public void SaveToFile(string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("--- Logs ---");
        foreach (var l in Logs)
        {
            sb.AppendLine(l);
        }

        sb.AppendLine("--- Errors ---");
        foreach (var l in ErrorLogs)
        {
            sb.AppendLine(l);
        }

        System.IO.File.WriteAllText(path, sb.ToString());
    }
}

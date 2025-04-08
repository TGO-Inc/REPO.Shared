using BepInEx.Logging;
using Sentry;
using Sentry.Extensibility;

namespace Shared.Core.Converters;

public class BepLog2SenLog(ManualLogSource logSource, SentryLevel senLogLevel) : IDiagnosticLogger
{
    public bool IsEnabled(SentryLevel level) => level >= senLogLevel;

    public void Log(SentryLevel logLevel, string message, Exception? exception = null, params object?[] args)
    {
        logSource.Log(logLevel switch
        {
            SentryLevel.Debug => LogLevel.Debug,
            SentryLevel.Info => LogLevel.Info,
            SentryLevel.Warning => LogLevel.Warning,
            SentryLevel.Error => LogLevel.Error,
            SentryLevel.Fatal => LogLevel.Fatal,
            _ => LogLevel.Debug
        }, $"Sentry: ({logLevel.ToString()}) {string.Format(message, args)} {exception}");
    }
}
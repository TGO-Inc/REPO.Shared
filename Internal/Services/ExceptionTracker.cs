using System.Collections.Concurrent;
using Shared.Core.Converters;
using UnityEngine;

namespace Shared.Internal.Services;

internal static class ExceptionTracker
{
    internal static event Action<Exception, LogType>? OnException;
    private static readonly ConcurrentDictionary<string, Exception> Exceptions = [];

    internal static void Init()
    {
        Application.logMessageReceived += InternalLogTracker;
    }
    
    internal static void RegisterException(string key, Exception exception)
    {
        Exceptions.TryAdd(key, exception);
    }
    
    private static void InternalLogTracker(string condition, string stackTrace, LogType type)
    {
        if (type is LogType.Log or LogType.Warning or LogType.Assert)
            return;
        
        stackTrace = ExceptionStackTraceStringConverter.SimplifyUnityStackTraceString(stackTrace);
        if (Exceptions.TryRemove(stackTrace, out var exception))
            OnException?.Invoke(exception, type);
    }
}
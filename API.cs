using System;
using System.Collections.Concurrent;
using Shared.Internal;
using UnityEngine;

namespace Shared;

public static class API
{
    /// <summary>
    /// Event that is triggered when an exception occurs.
    /// </summary>
    public static event Action<Exception, LogType>? OnException;

    internal static readonly ConcurrentDictionary<string, Exception> Exceptions = [];
    static API() { Application.logMessageReceived += Log; }
    private static void Log(string condition, string stackTrace, LogType type)
    {
        if (type is LogType.Log or LogType.Warning or LogType.Assert)
            return;
        
        stackTrace = ExceptionStackTraceStringConverter.SimplifyUnityStackTraceString(stackTrace);
        if (Exceptions.TryRemove(stackTrace, out var exception))
            OnException?.Invoke(exception, type);
    }
}
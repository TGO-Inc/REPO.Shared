using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    
    public static IReadOnlyDictionary<AssemblyName, Assembly> LoadedAssemblies => AssemblyResolver.LoadedAssemblies;
    public static IReadOnlyDictionary<string, AssemblyDependency> Dependencies => AssemblyResolver.Dependencies;
    
    internal static void Log(string condition, string stackTrace, LogType type)
    {
        if (type is LogType.Log or LogType.Warning or LogType.Assert)
            return;
        
        stackTrace = ExceptionStackTraceStringConverter.SimplifyUnityStackTraceString(stackTrace);
        if (Exceptions.TryRemove(stackTrace, out var exception))
            OnException?.Invoke(exception, type);
    }
}
using System.Reflection;
using Shared.Core.Models.AssemblyModels;
using Shared.Internal.Services;
using UnityEngine;

namespace Shared.Core;

public static class API
{
    /// <summary>
    /// Event that is triggered when an exception occurs.
    /// </summary>
    public static event Action<Exception, LogType>? OnException;
    
    /// <summary>
    /// The current <b>manually</b> loaded assemblies.
    /// </summary>
    public static IEnumerable<Assembly> LoadedAssemblies => AssemblyResolver.LoadedAssemblies;
    
    /// <summary>
    /// A read-only copy of the dependency graph.
    /// </summary>
    public static IReadOnlyAssemblyDependencyGraph Dependencies => AssemblyResolver.Dependencies;

    internal static void Init()
    {
        // Initialize the ExceptionTracker
        ExceptionTracker.OnException += OnException;
        ExceptionTracker.Init();
        
        // Initialize the AssemblyResolver
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolve;
    }
}
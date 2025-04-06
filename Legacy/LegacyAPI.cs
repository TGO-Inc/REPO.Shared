using System.Reflection;

namespace Shared.Legacy;

[Obsolete("This class only exists to preserve old code.")]
internal static class LegacyAPI
{
    public static IReadOnlyDictionary<AssemblyName, Assembly> LoadedAssemblies => AssemblyResolver.LoadedAssemblies;
    public static IReadOnlyDictionary<string, AssemblyDependency> Dependencies => AssemblyResolver.Dependencies;
    
    private static void _()
    {
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolve;
    }
}
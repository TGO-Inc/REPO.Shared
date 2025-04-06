using System.Reflection;

namespace Shared.Legacy;

internal class EmbeddedAssembly(AssemblyName asmName, string path)
{
    public readonly AssemblyName AssemblyName = asmName;
    public readonly string ResourcePath = path;
}
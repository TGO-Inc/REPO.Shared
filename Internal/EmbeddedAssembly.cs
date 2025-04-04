using System.Reflection;

namespace Shared.Internal;

public class EmbeddedAssembly(AssemblyName asmName, string path)
{
    public readonly AssemblyName AssemblyName = asmName;
    public readonly string ResourcePath = path;
}
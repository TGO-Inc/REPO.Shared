using System.Reflection;

namespace Shared.Legacy;

internal class AssemblyDependency(AssemblyName parent, AssemblyName child)
{
    public readonly AssemblyName Parent = parent;
    public readonly AssemblyName Child = child;
}
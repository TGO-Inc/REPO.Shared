using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shared.Internal;

public class AssemblyDependency(AssemblyName parent, AssemblyName child)
{
    public readonly AssemblyName Parent = parent;
    public readonly AssemblyName Child = child;
}
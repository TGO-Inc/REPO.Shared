using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

internal class EmbeddedAssembly(ISimpleAssemblyName asmName, Assembly resourceAsm, string path) 
    : SimpleAssemblyName(asmName), IEmbeddedAssembly
{
    public string ResourcePath { get; } = path;
    
    public Assembly ResourceAssembly { get; } = resourceAsm;
}
using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public interface IEmbeddedAssembly
{
    /// <summary>
    /// The path in the assembly where the resource is located.
    /// </summary>
    public string ResourcePath { get; }
    
    /// <summary>
    /// The assembly that contains the resource.
    /// </summary>
    public Assembly ResourceAssembly { get; }
}
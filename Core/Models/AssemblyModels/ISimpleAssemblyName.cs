using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public interface ISimpleAssemblyName
    : IEquatable<ISimpleAssemblyName>,
    IEquatable<string>,
    IEquatable<Assembly>,
    IEquatable<AssemblyName>
{
    /// <summary>
    /// The name of the assembly.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The version of the assembly as a string.
    /// </summary>
    public string StrVersion { get; }
    
    /// <summary>
    /// The version of the assembly.
    /// </summary>
    public System.Version? Version { get;  }
    
    /// <summary>
    /// The public key token of the assembly.
    /// </summary>
    public byte[] PublicKeyToken { get; }
    
    /// <summary>
    /// The public key of the assembly.
    /// </summary>
    public byte[] PublicKey { get; }
    
    /// <summary>
    /// Serializes the assembly name to a string.
    /// </summary>
    /// <returns></returns>
    public string SerializeToString();
}
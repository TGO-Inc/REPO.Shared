using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public class AssemblyIndexer : 
    IDependencyIndexer<string, ISimpleAssemblyName, Assembly>,
    IEquatable<AssemblyIndexer>,
    IEquatable<string>,
    IEquatable<ISimpleAssemblyName>,
    IEquatable<Assembly>
{
    private readonly Assembly? _assembly;
    private readonly ISimpleAssemblyName? _assemblyName;
    
    /// <summary>
    /// Safe [ unique, not null, insensitive ]
    /// </summary>
    private readonly string _assemblyNameStr;
    
    public AssemblyIndexer(string assemblyFullName)
    {
        _assemblyNameStr = assemblyFullName;
    }

    public AssemblyIndexer(Assembly assembly)
    {
        _assembly = assembly;
        _assemblyName = new SimpleAssemblyName(assembly.GetName());
        _assemblyNameStr = _assemblyName.SerializeToString();
    }
    
    public AssemblyIndexer(ISimpleAssemblyName assemblyName)
    {
        _assemblyName = assemblyName;
        _assemblyNameStr = assemblyName.SerializeToString();
    }

    public override string ToString() => _assemblyNameStr;

    public bool Equals(string? obj)
        => obj is not null && _assemblyNameStr.Equals(obj, StringComparison.Ordinal);
    
    public bool Equals(ISimpleAssemblyName? obj) 
        => obj is not null && Equals(obj, StringComparison.Ordinal);
    public bool Equals(ISimpleAssemblyName obj, StringComparison comp)
        => _assemblyName?.Equals(obj) == true || _assemblyNameStr.Equals(obj.SerializeToString(), comp);
    
    public bool Equals(Assembly? obj)
        => obj is not null && Equals(obj, StringComparison.Ordinal);
    
    public bool Equals(Assembly obj, StringComparison comp)
        => _assembly == obj || Equals(new SimpleAssemblyName(obj.GetName()), comp);

    public bool Equals(AssemblyIndexer? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || Equals(other._assemblyNameStr);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is AssemblyIndexer ind && Equals(ind);
    }

    public override int GetHashCode() => _assemblyNameStr.GetHashCode();
}
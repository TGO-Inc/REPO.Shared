using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public class AssemblyBranch(AssemblyIndexer id) 
    : IDependencyBranch<AssemblyIndexer, Assembly, AssemblyBranch>,
        IEquatable<AssemblyBranch>,
        IEquatable<AssemblyIndexer>
{
    public AssemblyIndexer Id { get; } = id;
    public Assembly? Value { get; private set; }
    public IEnumerable<AssemblyBranch> Parents => _parents.Keys;
    public IEnumerable<AssemblyBranch> Children => _children.Keys;

    private readonly ConcurrentDictionary<AssemblyBranch, byte> _children = [];
    private readonly ConcurrentDictionary<AssemblyBranch, byte> _parents = [];

    public AssemblyBranch(AssemblyIndexer id, Assembly value) : this(id)
    {
        Value = value;
    }
    
    public AssemblyBranch(AssemblyIndexer id, Assembly value, params AssemblyBranch[] parents) : this(id, value)
    {
        foreach (var parent in parents)
            _parents.TryAdd(parent, 0);
    }
    
    public AssemblyBranch(AssemblyIndexer id, params AssemblyBranch[] parents) : this(id)
    {
        foreach (var parent in parents)
            _parents.TryAdd(parent, 0);
    }
    
    public bool TrySetValue(Assembly value)
    {
        if (Value is not null)
            return false;
        Value = value;
        return true;
    }

    public void SetValue(Assembly value) => Value = value;
    
    public void SafeSetValue(Assembly value) => Value ??= value;

    public bool TryAddChild(AssemblyBranch child) => _children.TryAdd(child, 0);
    
    public bool TryAddParent(AssemblyBranch parent) => _parents.TryAdd(parent, 0);

    public bool Equals(AssemblyBranch other) => Id.Equals(other.Id);

    public bool Equals(AssemblyIndexer other) => other.Equals(Id);

    public override bool Equals(object? obj) => obj is AssemblyBranch other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();
}
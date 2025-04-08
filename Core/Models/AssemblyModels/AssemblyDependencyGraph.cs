using System.Collections.Concurrent;
using System.Text;
using Shared.Extensions;
using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public class AssemblyDependencyGraph : 
    IReadOnlyAssemblyDependencyGraph,
    IDependencyGraph<AssemblyIndexer, AssemblyBranch, Assembly>
{
    private readonly ConcurrentDictionary<AssemblyIndexer, AssemblyBranch> _branches = [];

    /// <summary>
    /// Gets all the underlying assemblies
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Assembly> GetValues()
        => _branches.Values.Where(branch => branch.Value is not null).Select(branch => branch.Value!);
    
    /// <summary>
    /// Gets all the dependencies of the root branch.
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetDirectDependencies(AssemblyIndexer root)
        => GetDependencies(root, root, 0);

    /// <summary>
    /// Gets all the dependencies of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetAllDependencies(AssemblyIndexer root)
        => GetDependencies(root, root, int.MaxValue);

    /// <summary>
    /// Gets all the dependencies of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetDependencies(AssemblyIndexer root, int depth = 0)
        => GetDependencies(root, root, depth);

    /// <summary>
    /// Gets all the dependencies of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="branchStart"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetAllDependencies(AssemblyIndexer root, AssemblyIndexer branchStart)
        => root.Equals(branchStart) ? [] : GetDependencies(root, branchStart, int.MaxValue);

    /// <summary>
    /// Gets all the dependencies of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="branchStart"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetDependencies(AssemblyIndexer root, AssemblyIndexer branchStart, int depth)
    {
        if (depth < 0) yield break;
        if (!_branches.TryGetValue(root, out var branch)) yield break;
        depth--;
        
        foreach (var child in branch.Children)
        {
            if (root.Equals(child.Id) || branchStart.Equals(child.Id))
                continue;

            yield return child.Id;
            foreach (var asm in GetDependencies(child.Id, branchStart, depth))
                yield return asm;
        }
    }

    /// <summary>
    /// Gets all the parents of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetAllParents(AssemblyIndexer root)
        => GetParents(root, root, int.MaxValue);

    /// <summary>
    /// Gets all the parents of the root branch.
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetDirectParents(AssemblyIndexer root)
        => GetParents(root, root, 0);

    /// <summary>
    /// Gets all the parents of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetParents(AssemblyIndexer root, int depth = 0)
        => GetParents(root, root, int.MaxValue);

    /// <summary>
    /// Gets all the parents of the root branch recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="branchStart"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public IEnumerable<AssemblyIndexer> GetParents(AssemblyIndexer root, AssemblyIndexer branchStart, int depth)
    {
        if (depth < 0) yield break;
        if (!_branches.TryGetValue(root, out var branch)) yield break;
        depth--;
        
        foreach (var parent in branch.Parents)
        {
            if (root.Equals(parent.Id) || branchStart.Equals(parent.Id))
                continue;

            yield return parent.Id;
            foreach (var asm in GetParents(parent.Id, branchStart, depth))
                yield return asm;
        }
    }

    /// <summary>
    /// Checks if the graph contains a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(AssemblyIndexer key) => _branches.ContainsKey(key);

    /// <summary>
    /// Checks if the root branch contains the parent.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsParent(AssemblyIndexer root, AssemblyIndexer key)
        => ContainsParent(root, key, root, int.MaxValue);

    /// <summary>
    /// Checks if the root branch contains the parent recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsParent(AssemblyIndexer root, AssemblyIndexer key, int depth)
        => ContainsParent(root, key, root, depth);
    
    /// <summary>
    /// Checks if the root branch contains the parent recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="branchStart"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsParent(AssemblyIndexer root, AssemblyIndexer key, AssemblyIndexer branchStart, int depth)
    {
        if (depth < 0) return false;
        if (!_branches.TryGetValue(root, out var branch)) return false;
        depth--;
        
        foreach (var parent in branch.Parents)
        {
            if (root.Equals(parent.Id) || branchStart.Equals(parent.Id))
                continue;

            if (parent.Id.Equals(key)) return true;
            if (ContainsParent(parent.Id, key, branchStart, depth)) return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the root branch contains the parent.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsDirectParent(AssemblyIndexer root, AssemblyIndexer key)
        => ContainsParent(root, key, root, 0);

    /// <summary>
    /// Checks if the root branch contains the child recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsChild(AssemblyIndexer root, AssemblyIndexer key)
        => ContainsChild(root, key, root, int.MaxValue);

    /// <summary>
    /// Checks if the root branch contains the child.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsDirectChild(AssemblyIndexer root, AssemblyIndexer key, int depth)
        => ContainsChild(root, key, root, depth);

    /// <summary>
    /// Checks if the root branch contains the child recursively.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="branchStart"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsChild(AssemblyIndexer root, AssemblyIndexer key, AssemblyIndexer branchStart, int depth)
    {
        if (root.Equals(branchStart)) return true;
        if (depth < 0) return false;
        if (!_branches.TryGetValue(root, out var branch)) return false;
        depth--;
        
        foreach (var child in branch.Children)
        {
            if (root.Equals(child.Id) || branchStart.Equals(child.Id))
                continue;

            if (child.Id.Equals(key)) return true;
            if (ContainsChild(child.Id, key, branchStart, depth)) return true;
        }
        
        return false;
    }

    /// <summary>
    /// Tries to get the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Assembly? GetValueOf(AssemblyIndexer key)
        => _branches.TryGetValue(key, out var branch) ? branch.Value : null;

    /// <summary>
    /// Tries to get the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetValueOf(AssemblyIndexer key, out Assembly value)
        => (value = GetValueOf(key)!) is not null;

    /// <summary>
    /// Links a parent to its child and vice versa.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public bool LinkBranchTo(Assembly parent, ISimpleAssemblyName child)
    {
        var parentIndexer = new AssemblyIndexer(parent);
        var childIndexer = new AssemblyIndexer(child);
        return LinkBranchTo(parentIndexer, childIndexer);
    }
    
    /// <summary>
    /// Links a parent to its child and vice versa.
    /// </summary>
    /// <param name="parentIndexer"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public bool LinkBranchTo(AssemblyIndexer parentIndexer, ISimpleAssemblyName child)
    {
        var childIndexer = new AssemblyIndexer(child);
        return LinkBranchTo(parentIndexer, childIndexer);
    }

    /// <summary>
    /// Links a parent to its child and vice versa.
    /// </summary>
    /// <param name="parentIndexer"></param>
    /// <param name="childIndexer"></param>
    /// <returns></returns>
    public bool LinkBranchTo(AssemblyIndexer parentIndexer, AssemblyIndexer childIndexer)
    {
        // Sanity check
        if (parentIndexer.Equals(childIndexer))
            return false;
        
        // Ensure the parent branch exists
        if (!_branches.TryGetValue(parentIndexer, out var parentBranch))
        {
            parentBranch = new AssemblyBranch(parentIndexer);
            _branches.TryAdd(parentBranch.Id, parentBranch);
        }
        
        // Ensure the child branch exists
        if (!_branches.TryGetValue(childIndexer, out var childBranch))
        {
            childBranch = new AssemblyBranch(childIndexer);
            _branches.TryAdd(childBranch.Id, childBranch);
        }

        // Link the branches
        return parentBranch.TryAddChild(childBranch) && childBranch.TryAddParent(parentBranch);
    }

    /// <summary>
    /// Links a branch to its parents and children.
    /// </summary>
    /// <param name="branch"></param>
    /// <returns></returns>
    public bool LinkBranch(AssemblyBranch branch)
    {
        if (!EnsureBranch(branch, out var existingBranch))
            return false;

        foreach (var child in existingBranch.Children)
            if (EnsureBranch(child))
                LinkBranchTo(existingBranch.Id, child.Id);
        
        foreach (var parent in existingBranch.Parents)
            if (EnsureBranch(parent))
                LinkBranchTo(parent.Id, existingBranch.Id);
            
        return true;
    }

    /// <summary>
    /// Ensures that the branch exists in the graph and sets its value if it does not exist.
    /// </summary>
    /// <param name="branch"></param>
    /// <returns></returns>
    public bool EnsureBranch(AssemblyBranch branch)
        => (_branches.TryGetValue(branch.Id, out var existingBranch) || _branches.TryAdd(branch.Id, branch))
           && (branch.Value is null || existingBranch is null || existingBranch.TrySetValue(branch.Value));
    
    /// <summary>
    /// Ensures that the branch exists in the graph and sets its value if it does not exist.
    /// </summary>
    /// <param name="branch"></param>
    /// <param name="existingBranch"></param>
    /// <returns></returns>
    public bool EnsureBranch(AssemblyBranch branch, out AssemblyBranch existingBranch)
        => (_branches.TryGetValue(branch.Id, out existingBranch) || 
            (_branches.TryAdd(branch.Id, branch) && (existingBranch = branch) is not null))
           && (branch.Value is null || existingBranch is null || existingBranch.TrySetValue(branch.Value));

    /// <summary>
    /// Sets the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void SetValueOfBranch(AssemblyBranch key, Assembly value)
        => SetValueOfBranch(key.Id, value);

    /// <summary>
    /// Sets the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void SetValueOfBranch(AssemblyIndexer key, Assembly value)
    {
        if (!_branches.TryGetValue(key, out var branch))
            throw new KeyNotFoundException($"Branch {key} not found in the graph.");
        
        branch.SetValue(value);
    }

    /// <summary>
    /// Tries to set the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TrySetValueOfBranch(ISimpleAssemblyName key, Assembly value)
        => TrySetValueOfBranch(new AssemblyIndexer(key), value);
    
    /// <summary>
    /// Tries to set the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TrySetValueOfBranch(AssemblyBranch key, Assembly value)
        => TrySetValueOfBranch(key.Id, value);

    /// <summary>
    /// Tries to set the value of a branch.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TrySetValueOfBranch(AssemblyIndexer key, Assembly value)
        => _branches.TryGetValue(key, out var branch) && branch.TrySetValue(value);
    
    /// <summary>
    /// Prints the graph in a human-readable format.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string PrintGraph()
    {
        var sb = new StringBuilder();
        sb.AddWithNewLine("\n===================Dependency=Graph======================");
        foreach (var branch in _branches.Values)
        {
            sb.AddWithNewLine($"Assembly: {branch.Id}");
            sb.AddWithNewLine("Needed by:");
            foreach (var parent in branch.Parents)
                sb.AddWithNewLine($"  - {parent.Id}");
            sb.AddWithNewLine("Depends on:");
            foreach (var child in branch.Children)
                sb.AddWithNewLine($"  - {child.Id}");
            sb.AddWithNewLine("=========================================================");
        }
        return sb.ToString();
    }
}
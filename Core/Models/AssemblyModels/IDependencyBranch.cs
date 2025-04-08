namespace Shared.Core.Models.AssemblyModels;

public interface IDependencyBranch<TIndexer, TValue, TRoot> 
    : IEquatable<TIndexer>
    where TIndexer : IDependencyIndexer
    where TRoot : IDependencyBranch<TIndexer, TValue, TRoot>
{
    /// <summary>
    /// Gets the indexer of the branch
    /// </summary>
    TIndexer Id { get; }
    
    /// <summary>
    /// Gets the value of the branch
    /// </summary>
    TValue? Value { get; }
    
    /// <summary>
    /// Gets the parents of the branch
    /// </summary>
    IEnumerable<TRoot> Parents { get; }
    
    /// <summary>
    /// Gets the children of the branch
    /// </summary>
    IEnumerable<TRoot> Children { get; }
    
    /// <summary>
    /// Tries to get the value of the branch
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TrySetValue(TValue value);
    
    /// <summary>
    /// Sets the value of the branch
    /// </summary>
    /// <param name="value"></param>
    void SetValue(TValue value);
    
    /// <summary>
    /// Tries to add a child to the branch
    /// </summary>
    /// <param name="child"></param>
    /// <returns></returns>
    bool TryAddChild(TRoot child);
    
    /// <summary>
    /// Tries to add a parent to the branch
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    bool TryAddParent(TRoot parent);
}
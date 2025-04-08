namespace Shared.Core.Models.AssemblyModels;

public interface IDependencyGraph<TIndexer, in TBranch, TValue> : IReadOnlyDependencyGraph<TIndexer, TValue> 
    where TIndexer : IDependencyIndexer
    where TBranch : IDependencyBranch<TIndexer, TValue, TBranch>
{
    /// <summary>
    /// Adds a child to the parent branch
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public bool LinkBranchTo(TIndexer parent, TIndexer child);

    /// <summary>
    /// Links a branch to the graph
    /// </summary>
    /// <param name="branch"></param>
    /// <returns></returns>
    public bool LinkBranch(TBranch branch);
    
    /// <summary>
    /// Sets the value of a branch
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValueOfBranch(TBranch key, TValue value);
    
    /// <summary>
    /// Sets the value of a branch
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValueOfBranch(TIndexer key, TValue value);
    
    /// <summary>
    /// Tries to set the value of a branch
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TrySetValueOfBranch(TBranch key, TValue value);
    
    /// <summary>
    /// Tries to set the value of a branch
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TrySetValueOfBranch(TIndexer key, TValue value);
}
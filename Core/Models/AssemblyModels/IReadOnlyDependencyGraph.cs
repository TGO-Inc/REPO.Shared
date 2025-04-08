namespace Shared.Core.Models.AssemblyModels;

public interface IReadOnlyDependencyGraph<TIndexer, TValue> where TIndexer : IDependencyIndexer
{
    /// <summary>
    /// Gets only the direct dependencies
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetDirectDependencies(TIndexer root);
    
    /// <summary>
    /// Gets all dependencies (recursively)
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetAllDependencies(TIndexer root);
    
    /// <summary>
    /// Gets all dependencies (recursively) with a depth limit
    /// </summary>
    /// <param name="root"></param>
    /// <param name="depth">depth &lt; 0 returns no dependencies</param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetDependencies(TIndexer root, int depth);
    
    /// <summary>
    /// Gets all the parent dependencies (recursively)
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetAllParents(TIndexer root);
    
    /// <summary>
    /// Gets only the direct parent dependencies
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetDirectParents(TIndexer root);
    
    /// <summary>
    /// Gets all the parent dependencies (recursively) with a depth limit
    /// </summary>
    /// <param name="root"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public IEnumerable<TIndexer> GetParents(TIndexer root, int depth);
    
    /// <summary>
    /// Checks to see if the tree contains a branch with the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TIndexer key);

    /// <summary>
    /// Checks if the tree contains a parent of the given key
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsParent(TIndexer root, TIndexer key);
    
    /// <summary>
    /// Checks if the tree contains a parent of the given key with a depth limit
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsParent(TIndexer root, TIndexer key, int depth);
    
    /// <summary>
    /// Checks if the tree contains a direct parent of the given key
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsDirectParent(TIndexer root, TIndexer key);
    
    /// <summary>
    /// Checks if the tree contains a child of the given key
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsChild(TIndexer root, TIndexer key);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="root"></param>
    /// <param name="key"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public bool ContainsDirectChild(TIndexer root, TIndexer key, int depth);
    
    /// <summary>
    /// Gets the underlying value of the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue? GetValueOf(TIndexer key);

    /// <summary>
    /// Gets the underlying value of the given key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetValueOf(TIndexer key, out TValue value);
    
    /// <summary>
    /// Prints the graph in a human-readable format
    /// </summary>
    /// <returns></returns>
    public string PrintGraph();
}
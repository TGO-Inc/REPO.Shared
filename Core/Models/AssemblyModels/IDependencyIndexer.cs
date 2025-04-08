namespace Shared.Core.Models.AssemblyModels;

public interface IDependencyIndexer { }

public interface IDependencyIndexer<in TOne> : IDependencyIndexer
{
    public bool Equals(TOne? obj);
}

public interface IDependencyIndexer<in TOne, in TTwo> : IDependencyIndexer<TOne>
{
    public bool Equals(TTwo? obj);
}

public interface IDependencyIndexer<in TOne, in TTwo, in TThree> : IDependencyIndexer<TOne, TTwo>
{
    public bool Equals(TThree? obj);
}
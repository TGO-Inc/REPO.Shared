using System.Reflection;

namespace Shared.Core.Models.AssemblyModels;

public interface IReadOnlyAssemblyDependencyGraph : IReadOnlyDependencyGraph<AssemblyIndexer, Assembly> { }
using System.IO.Compression;
using System.Reflection;
using Shared.Core.Models.AssemblyModels;

namespace Shared.Internal.Services;

internal static class AssemblyResolver
{
    internal static readonly AssemblyDependencyGraph Dependencies = new();
    internal static IEnumerable<Assembly> LoadedAssemblies => Dependencies.GetValues();
    
    internal static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        if (!SimpleAssemblyName.HasNameAndVersion(args.Name))
            return null!;
        
        try
        {
            var taSimpleName = SimpleAssemblyName.DeserializeNormal(args.Name);
            var raSimpleName = new SimpleAssemblyName(args.RequestingAssembly);
            var raIndexer = new AssemblyIndexer(raSimpleName);
            var taIndexer = new AssemblyIndexer(taSimpleName);
            List<AssemblyIndexer> searchAssemblies = [raIndexer];
            
            // Document the requesting assembly and its dependency
            var requestingBranch = new AssemblyBranch(raIndexer, args.RequestingAssembly);
            var targetBranch = new AssemblyBranch(taIndexer, requestingBranch);
            Dependencies.LinkBranch(targetBranch);
            Dependencies.LinkBranch(requestingBranch);
            
            // Check to see if the requesting assembly is a dependency of another assembly
            if (Dependencies.Contains(raIndexer))
            {
                // Now we want to search the assembly resources upwards
                searchAssemblies.AddRange(Dependencies.GetAllParents(raIndexer));
            }

            foreach (var currentIndexer in searchAssemblies)
            {
                if (!Dependencies.GetValueOf(currentIndexer, out var loadedAssembly))
                    continue;
                
                // Try to load the assembly from the embedded resources by path name
                var filteredEmbeddedAssemblies = GetAllEmbeddedAssemblies(loadedAssembly).Where(taSimpleName.Equals);
                foreach (var filteredEmbeddedAssembly in filteredEmbeddedAssemblies)
                {
                    var res = DecompressAndLoadAssembly(loadedAssembly, raIndexer,
                        filteredEmbeddedAssembly);
                    if (res is not null) return res;
                }
            }

            throw new DllNotFoundException(
                $"Failed to resolve assembly: '{taSimpleName.SerializeToString()}' for '{args.RequestingAssembly.FullName}'");
        }
        catch (Exception ex)
        {
            // Probably nothing... no need to concern the user (Debug)
            Entry.LogSource.LogWarning(ex);
            Entry.CaptureException(ex);
        }

        return null!;
    }
    
    private static IEnumerable<EmbeddedAssembly> GetAllEmbeddedAssemblies(Assembly assembly)
    {
        var resources = assembly.GetManifestResourceNames()
                                            .Where(n => n.StartsWith("BundledAssemblies"))
                                            .ToArray();
        
        foreach (var path in resources)
        {
            if (!path.EndsWith(".dll.gz"))
                continue;

            // Remove the ".gz" suffix to get the original path
            var realPath = path.Substring(0, path.Length - 3);
            if (!resources.Contains(realPath + ".dllmeta")) continue;

            var metaStream = assembly.GetManifestResourceStream(realPath + ".dllmeta");
            if (metaStream == null) continue;

            using var reader = new BinaryReader(metaStream);
            var metaData = reader.ReadBytes((int)metaStream.Length);
            var name = SimpleAssemblyName.DeserializeFromData(metaData);
            yield return new EmbeddedAssembly(name, assembly, path); 
        }
    }

    private static Assembly? DecompressAndLoadAssembly(Assembly assembly, AssemblyIndexer requestingId, EmbeddedAssembly embeddedAssembly)
    {
        // Load the embedded resource as a stream
        using var stream = assembly.GetManifestResourceStream(embeddedAssembly.ResourcePath);
        if (stream == null)
            throw new FileNotFoundException($"Resource '{embeddedAssembly.ResourcePath}' not found in assembly '{assembly.FullName}'.");
        
        // Decompress the stream
        using var decompressedStream = new MemoryStream();
        using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
        gZipStream.CopyTo(decompressedStream);
        decompressedStream.Position = 0;
        
        // Document the assembly as a dependency
        Dependencies.LinkBranchTo(requestingId, embeddedAssembly);

        try
        {
            // Load the assembly from the decompressed stream
            var assemblyData = decompressedStream.ToArray();
            var loadedAssembly = Assembly.Load(assemblyData);
            
            // Update the assembly value
            Dependencies.TrySetValueOfBranch(embeddedAssembly, loadedAssembly);
            Entry.LogSource.LogInfo($"Loaded assembly: '{embeddedAssembly.Name}' from '{embeddedAssembly.ResourcePath}'");
            
            // Return the loaded assembly
            return loadedAssembly;
        }
        catch (BadImageFormatException ex)
        {
            // probably okay
            return null;
        }
        catch (Exception ex)
        {
            // erm..
            Entry.LogSource.LogWarning(ex);
            Entry.CaptureException(ex);
            return null;
        }
    }
}
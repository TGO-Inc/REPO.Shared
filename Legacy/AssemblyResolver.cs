using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Shared.Internal;

namespace Shared.Legacy;

internal static class AssemblyResolver
{
    internal static readonly ConcurrentDictionary<string, AssemblyDependency> Dependencies = [];
    internal static readonly ConcurrentDictionary<AssemblyName, Assembly> LoadedAssemblies = [];
    
    internal static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var targetAssemblyNameStr = args.Name.Split(',')[0].ToLowerInvariant();
        var requestingAssemblyName = args.RequestingAssembly.GetName();
        var requestingAssemblyNameStr = SerializeAssemblyName(requestingAssemblyName);
        AssemblyName[] searchAssemblies = [requestingAssemblyName];
        
        // Document the requesting assembly as loaded
        LoadedAssemblies.TryAdd(requestingAssemblyName, args.RequestingAssembly);
        
        // Check to see if the requesting assembly is a dependency of another assembly
        if (Dependencies.ContainsKey(requestingAssemblyNameStr))
        {
            // Now we want to search the assembly resources upwards
            searchAssemblies = GetAllParents(requestingAssemblyName).ToArray();
        }

        foreach (var assemblyName in searchAssemblies)
        {
            if (!LoadedAssemblies.TryGetValue(assemblyName, out var loadedAssembly))
                continue;

            var embeddedAssemblies = GetAllEmbeddedAssemblies(loadedAssembly).ToArray();
            // Entry.LogSource.LogWarning("Embedded assemblies:\n" + string.Join("\n", embeddedAssemblies.Select(e => e.ResourcePath)));
            
            // Ensure that each assembly is documented as a dependency of the loaded assembly
            foreach (var eAsm in embeddedAssemblies)
                Dependencies.TryAdd(SerializeAssemblyName(eAsm.AssemblyName), new AssemblyDependency(assemblyName, eAsm.AssemblyName));
            
            // Try to load the assembly from the embedded resources by path name
            var filteredEmbeddedAssemblies = embeddedAssemblies
                .Where(e => e.ResourcePath.ToLowerInvariant().EndsWith($"{targetAssemblyNameStr}.dll.gz"));


            foreach (var filteredEmbeddedAssembly in filteredEmbeddedAssemblies)
            {
                var res = DecompressAndLoadAssembly(loadedAssembly, filteredEmbeddedAssembly);
                if (res is not null) return res;
            }
            
            // Entry.LogSource.LogError("Failed to load assembly: " + targetAssemblyNameStr);
        }
        
        Entry.LogSource.LogDebug($"Failed to locate assembly for load: {targetAssemblyNameStr} from {args.RequestingAssembly.FullName}");
        return null!;
    }
    
    private static IEnumerable<EmbeddedAssembly> GetAllEmbeddedAssemblies(Assembly assembly)
    {
        var resources = assembly.GetManifestResourceNames()
                                            .Where(n => n.StartsWith("BundledAssemblies"))
                                            .ToArray();
        
        foreach (var path in resources)
        {
            // Entry.LogSource.LogWarning("RESOURCES: " + path);
            if (path.EndsWith(".dllmeta"))
                continue;

            var realPath = path.Substring(0, path.Length - 3);
            if (!resources.Contains(realPath + ".dllmeta")) continue;
            
            var metaStream = assembly.GetManifestResourceStream(realPath + ".dllmeta");
            if (metaStream == null) continue;
            
            using var reader = new BinaryReader(metaStream);
            var metaData = reader.ReadBytes((int)metaStream.Length);
            var name = DeserializeAssemblyName(metaData);
            var embeddedAssembly = new EmbeddedAssembly(name, path);
            yield return embeddedAssembly;
        }
    }

    private static IEnumerable<AssemblyName> GetAllParents(AssemblyName assembly)
        => GetAllParents(new AssemblyDependency(assembly, null!));

    private static IEnumerable<AssemblyName> GetAllParents(AssemblyDependency assembly)
    {
        var parentAssembly = assembly.Parent;
        
        if (Dependencies.TryGetValue(SerializeAssemblyName(parentAssembly), out var dependencyItem))
            foreach (var asm in GetAllParents(dependencyItem))
                yield return asm;
        
        yield return parentAssembly;
    }

    private static Assembly? DecompressAndLoadAssembly(Assembly assembly, EmbeddedAssembly embeddedAssembly)
    {
        // Load the embedded resource as a stream
        using var stream = assembly.GetManifestResourceStream(embeddedAssembly.ResourcePath);
        if (stream == null)
        {
            Entry.LogSource.LogError($"Resource '{embeddedAssembly.ResourcePath}' not found in assembly '{assembly.FullName}'.");
            throw new FileNotFoundException($"Resource '{embeddedAssembly.ResourcePath}' not found in assembly '{assembly.FullName}'.");
        }
        
        // Decompress the stream
        using var decompressedStream = new MemoryStream();
        using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
        gZipStream.CopyTo(decompressedStream);
        decompressedStream.Position = 0;
        
        // Document the assembly as a dependency
        Dependencies.TryAdd(SerializeAssemblyName(embeddedAssembly.AssemblyName), new AssemblyDependency(assembly.GetName(), embeddedAssembly.AssemblyName));
        try
        {
            // Load the assembly from the decompressed stream
            var assemblyData = decompressedStream.ToArray();
            var loadedAssembly = Assembly.Load(assemblyData);
            Entry.LogSource.LogDebug($"Loaded embedded assembly: {loadedAssembly.FullName}");
            
            // Return the loaded assembly
            return loadedAssembly;
        }
        catch (BadImageFormatException ex)
        {
            return null;
        }
        catch (Exception ex)
        {
            Entry.LogSource.LogError($"Failed to load assembly '{embeddedAssembly.AssemblyName}' from resource '{embeddedAssembly.ResourcePath}': {ex.Message}");
            throw;
        }
    }
    
    private static AssemblyName DeserializeAssemblyName(byte[] data)
    {
        var str = Encoding.UTF8.GetString(data);
        var lines = str.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
        
        var assemblyName = new AssemblyName { Name = lines[0], };
        
        // Version must be set before other properties
        if (System.Version.TryParse(lines[3], out var version))
            assemblyName.Version = version;
        
        // Set flags
        if (Enum.TryParse<AssemblyNameFlags>(lines[2], out var flags))
            assemblyName.Flags = flags;
        
        // Set content type
        if (Enum.TryParse<AssemblyContentType>(lines[4], out var contentType))
            assemblyName.ContentType = contentType;
        
        // Set public key
        assemblyName.SetPublicKey(lines[5].Split([";"], StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray());
        
        // Set public key token
        assemblyName.SetPublicKeyToken(lines[6].Split([";"], StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray());
        
        return assemblyName;
    }
    
    private static string SerializeAssemblyName(AssemblyName assemblyName)
    {
        var sb = new StringBuilder();
        sb.Append(assemblyName.Name);
        sb.Append(assemblyName.FullName.Replace(", Culture=neutral", ""));
        sb.Append(assemblyName.Flags.ToString());
        sb.Append(assemblyName.Version);
        sb.Append(assemblyName.ContentType.ToString());
        sb.Append(string.Join(";", assemblyName.GetPublicKey()));
        sb.Append(string.Join(";", assemblyName.GetPublicKeyToken()));
        
        return sb.ToString();
    }
}
using System.Reflection;
using System.Text;
using Shared.Internal.Extensions;

namespace Shared.Core.Models.AssemblyModels;

public class SimpleAssemblyName(string name, System.Version? version, byte[] publicKey, byte[] publicKeyToken)
    : ISimpleAssemblyName
{
    public string Name { get; set; } = name;
    public string StrVersion => Version?.ToString() ?? "";
    public System.Version? Version { get; set; } = version;
    public byte[] PublicKeyToken { get; set; } = publicKeyToken;
    public byte[] PublicKey { get; set; } = publicKey;
    
    public SimpleAssemblyName(AssemblyName assemblyName) : 
        this(assemblyName.Name, assemblyName.Version, assemblyName.GetPublicKey(), assemblyName.GetPublicKeyToken()) { }
    public SimpleAssemblyName(Assembly assembly) : this(assembly.GetName()) { }
    
    public SimpleAssemblyName(ISimpleAssemblyName assemblyName) : 
        this(assemblyName.Name, assemblyName.Version, assemblyName.PublicKey, assemblyName.PublicKeyToken) { }
    
    /// <summary>
    /// Serializes the assembly name to a string.
    /// </summary>
    /// <returns></returns>
    public string SerializeToString() => $"{Name},{StrVersion},{PublicKey.ToHexStr()},{PublicKeyToken.ToHexStr()}";
    
    public bool Equals(ISimpleAssemblyName other)
    {
        var name = string.IsNullOrWhiteSpace(Name) || Name.Equals(other.Name, StringComparison.Ordinal);
        var version = Version is null || Version == other.Version;
        var publicKey = PublicKey.Length == 0 || other.PublicKey.Length == 0 || PublicKey.SequenceEqual(other.PublicKey);
        var publicKeyToken = PublicKeyToken.Length == 0 || other.PublicKeyToken.Length == 0 || PublicKeyToken.SequenceEqual(other.PublicKeyToken);
        return name && version && publicKey && publicKeyToken;
    }

    public bool Equals(string other)
    {
        if (string.IsNullOrWhiteSpace(other)) return false;
        if (Name.Equals(other, StringComparison.Ordinal)) 
            return Version is null && PublicKey.Length == 0 && PublicKeyToken.Length == 0;
        return SerializeToString().Equals(other, StringComparison.Ordinal);
    }

    public bool Equals(Assembly other)
        => new SimpleAssemblyName(other).Equals(this);

    public bool Equals(AssemblyName other)
        => new SimpleAssemblyName(other).Equals(this);
    
    /// <summary>
    /// Deserializes the assembly name from a byte array.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ISimpleAssemblyName DeserializeFromData(byte[] data)
        => Deserialize(Encoding.UTF8.GetString(data));
    
    /// <summary>
    /// Deserializes the assembly name from a string.
    /// </summary>
    /// <param name="simpleAssemblyNameText"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ISimpleAssemblyName Deserialize(string simpleAssemblyNameText)
    {
        var parts = simpleAssemblyNameText.Split(',', '\n');
        
        if (parts.Length < 4)
            throw new ArgumentException("Invalid assembly name format", nameof(simpleAssemblyNameText));
        
        var name = parts[0];
        var version = System.Version.TryParse(parts[1], out var versionObj) ? versionObj : new System.Version(0, 0);
        var publicKey = string.IsNullOrEmpty(parts[2]) ? [] : parts[2].FromHexToBytes();
        var publicKeyToken = string.IsNullOrEmpty(parts[3]) ? [] : parts[3].FromHexToBytes();
        
        return new SimpleAssemblyName(name, version, publicKey, publicKeyToken);
    }
    
    public static bool HasNameAndVersion(string simpleAssemblyNameText)
    {
        var parts = simpleAssemblyNameText.Split(',', '\n');
        return parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) 
            && System.Version.TryParse(parts[1].Split('=')[1], out _);
    }

    public static ISimpleAssemblyName DeserializeNormal(string normalAssemblyNameText)
    {
        var parts = normalAssemblyNameText.Split(',', '\n');
        
        var name = parts[0];
        byte[] publicKeyToken = [];
        
        if (parts.Length < 2)
            throw new ArgumentException("Invalid assembly name format", nameof(normalAssemblyNameText));
        
        var versionStr = parts[1].Split('=')[1];
        System.Version.TryParse(versionStr, out var version);

        if (parts.Length <= 3) 
            return new SimpleAssemblyName(name, version, [], publicKeyToken);
        
        // Skip culture and public key
        var publicKeyTokenStr = parts[3].Split('=')[1];
        publicKeyToken = string.IsNullOrEmpty(publicKeyTokenStr) || publicKeyTokenStr == "null"
            ? []
            : publicKeyTokenStr.FromHexToBytes();

        return new SimpleAssemblyName(name, version, [], publicKeyToken);
    }
}
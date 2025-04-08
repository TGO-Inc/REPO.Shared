namespace Shared.Internal.Extensions;

internal static class ByteExtensions
{
    public static string ToHexStr(this byte[] bytes)
        => BitConverter.ToString(bytes).Replace("-", string.Empty);
    
    public static byte[] FromHexToBytes(this string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Hex string must have an even length.");

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var byteValue = hex.Substring(i * 2, 2);
            bytes[i] = Convert.ToByte(byteValue, 16);
        }
        return bytes;
    }
}
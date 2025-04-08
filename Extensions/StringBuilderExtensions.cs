using System.Text;

namespace Shared.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AddWithNewLine(this StringBuilder sb, object? value)
        => sb.Append(value).Append('\n');
}
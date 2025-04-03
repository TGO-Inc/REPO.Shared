using System;
using System.Linq;
using System.Text;

namespace Shared.Internal;

public static class ExceptionStackTraceStringConverter
{
    public static string SimplifyUnityStackTraceString(string stackTrace)
        => stackTrace.Replace("\r", string.Empty).Trim();
    
    public static string ConvertToUnityStackTraceString(string stackTrace)
    {
        // Process the stack trace for Unity format
        var sb = new StringBuilder();
        var lines = stackTrace.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (line.StartsWith("  at "))
            {
                // Remove "  at " prefix
                var processedLine = line.Substring(5); 
                
                // Check if it matches the pattern [.*](.+)$ at the end
                var match = System.Text.RegularExpressions.Regex.Match(processedLine, @"\[\S+x\S+\] in (\S+)\s*$");
                if (match.Success)
                {
                    // Get the captured group
                    var capturedPart = match.Groups[1].Value;
                    
                    // Get the part before the match
                    var lineWithoutMatch = processedLine.Substring(0, match.Index);
                    
                    // Append the new format
                    sb.Append($"{lineWithoutMatch}(at {capturedPart})\n");
                }
                else
                {
                    // Just add the line without "  at "
                    sb.Append($"{processedLine}\n");
                }
            }
            else
            {
                // Include other lines unchanged
                sb.Append($"{line}\n");
            }
        }
        
        return sb.ToString().Trim();
    }
}
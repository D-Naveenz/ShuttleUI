using System.Diagnostics.CodeAnalysis;

namespace ShuttleUI.Helpers;

public static class StringExtensions
{
    public static bool TryToEnum<T>(this string value, [NotNullWhen(true)] out T? result) where T : Enum
    {
        object? _result;
        if (Enum.TryParse(typeof(T), value, true, out _result))
        {
            result = (T)_result;
            return true;
        }

        result = default;
        return false;
    }

    public static string GetPathChunk(this string pathStr)
    {
        // If pathStr is a complete path, do not want to get the chunk
        if (Path.Exists(pathStr))
        {
            return pathStr;
        }

        // Remove the leading / or \\
        if (pathStr.StartsWith('/') || pathStr.StartsWith('\\'))
        {
            pathStr = pathStr[1..];
        }

        return pathStr.Replace('/', '\\');
    }
}
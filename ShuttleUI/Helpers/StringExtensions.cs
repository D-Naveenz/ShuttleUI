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
}
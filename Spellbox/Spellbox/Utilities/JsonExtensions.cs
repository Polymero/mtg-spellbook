using System.Text.Json;

namespace Spellbox.Utilities;

public static class JsonExtensions
{
    public static string GetPropertyOrEmptyString(this JsonElement e, string name)
        => e.TryGetProperty(name, out var p) && p.ValueKind != JsonValueKind.Null
            ? p.GetString()!
            : "";

    public static int? GetPropertyOrNullInt(this JsonElement e, string name)
        => e.TryGetProperty(name, out var p) && p.TryGetInt32(out var i)
            ? i
            : null;

    public static decimal? GetPropertyOrNullDecimal(this JsonElement e, string name)
        => e.TryGetProperty(name, out var p) && p.TryGetDecimal(out var d)
            ? d
            : null;
}
namespace RiverLine.Api.Common;

public static class PhoneNumbers
{
    private static readonly Regex ValidPattern = new(@"^\+?[0-9]{7,15}$", RegexOptions.Compiled);

    public static string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var builder = new StringBuilder(raw.Length);
        foreach (var c in raw.Trim())
        {
            if (c is ' ' or '-' or '(' or ')' or ' ')
                continue;

            if (c is >= '٠' and <= '٩')        // ٠–٩
                builder.Append((char)('0' + (c - '٠')));
            else if (c is >= '۰' and <= '۹')   // ۰–۹
                builder.Append((char)('0' + (c - '۰')));
            else
                builder.Append(c);
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    public static bool IsValid(string? raw)
    {
        var normalized = Normalize(raw);
        return normalized is null || ValidPattern.IsMatch(normalized);
    }
}

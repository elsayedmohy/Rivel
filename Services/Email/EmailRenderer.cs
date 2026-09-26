namespace RiverLine.Api.Services.Email;

public static class EmailRenderer
{
    private static readonly CultureInfo Culture = new("ar-EG");
    private static readonly Regex Placeholder =
        new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);
 
    public static (string Subject, string Html,string text)? Render(EmailMessageDto message)
    {
        var template = EmailTemplates.Get(message.TemplateKey);
        if (template is null)
            return null;
 
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["recipientName"] = message.RecipientName
        };
 
        foreach (var (key, element) in message.Data)
            values[key] = Format(key, element);
 
        var body = Fill(template.Body, values);
        var html = EmailTemplates.Layout.Replace("{{content}}", body);
 
        var text = ToPlainText(body);   
        return (Fill(template.Subject, values), html, text);
    }
 
    private static string Fill(string template, IReadOnlyDictionary<string, string> values)
        => Placeholder.Replace(template, m =>
            values.TryGetValue(m.Groups[1].Value, out var v) ? HtmlEncoder.Default.Encode(v) : "");
 
    private static string Format(string key, JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.TryGetInt64(out var l)
            ? l.ToString("N0", Culture)
            : element.GetDouble().ToString("N2", Culture),

        JsonValueKind.True => "نعم",
        JsonValueKind.False => "لا",
        JsonValueKind.Null or JsonValueKind.Undefined => "",

        JsonValueKind.String when IsDateKey(key)
                                  && DateTime.TryParse(element.GetString(), CultureInfo.InvariantCulture,
                                      DateTimeStyles.RoundtripKind, out var date)
            => date.ToString("d MMMM yyyy", Culture),

        JsonValueKind.String => element.GetString() ?? "",

        _ => element.ToString()
    };
    
    private static bool IsDateKey(string key) =>
        key.EndsWith("Date", StringComparison.Ordinal) ||
        key.EndsWith("At", StringComparison.Ordinal);
    
    private static string ToPlainText(string html)
    {
        var text = Regex.Replace(html, @"<br\s*/?>|</p>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<[^>]+>", "");
        return System.Net.WebUtility.HtmlDecode(text).Trim();
    }
    
}
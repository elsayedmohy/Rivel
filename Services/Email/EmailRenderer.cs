namespace RiverLine.Api.Services.Email;

public static class EmailRenderer
{
    private static readonly CultureInfo Culture = new("ar-EG");
    private static readonly Regex Placeholder =
        new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);
 
    public static (string Subject, string Html)? Render(EmailMessageDto message)
    {
        var template = EmailTemplates.Get(message.TemplateKey);
        if (template is null)
            return null;
 
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["recipientName"] = message.RecipientName
        };
 
        foreach (var (key, element) in message.Data)
            values[key] = Format(element);
 
        var body = Fill(template.Body, values);
        var html = EmailTemplates.Layout.Replace("{{content}}", body);
 
        return (Fill(template.Subject, values), html);
    }
 
    private static string Fill(string template, IReadOnlyDictionary<string, string> values)
        => Placeholder.Replace(template, m =>
            values.TryGetValue(m.Groups[1].Value, out var v) ? HtmlEncoder.Default.Encode(v) : "");
 
    private static string Format(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.TryGetInt64(out var l)
            ? l.ToString("N0", Culture)
            : element.GetDouble().ToString("N2", Culture),
 
        JsonValueKind.True => "نعم",
        JsonValueKind.False => "لا",
        JsonValueKind.Null or JsonValueKind.Undefined => "",
 
        JsonValueKind.String => DateTime.TryParse(
            element.GetString(), CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var date)
            ? date.ToString("d MMMM yyyy", Culture)
            : element.GetString() ?? "",
 
        _ => element.ToString()
    };
}
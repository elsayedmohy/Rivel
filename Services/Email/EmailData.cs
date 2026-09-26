namespace RiverLine.Api.Services.Email;

public static class EmailData
{
    public static IReadOnlyDictionary<string, JsonElement> From(object data) =>
        JsonSerializer.SerializeToElement(data)
            .EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value);
}
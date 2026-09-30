namespace RiverLine.Api.Settings;

public class SupabaseStorageSettings
{
    public string Url { get; set; } = default!;
    public string BaseUrl => new Uri(Url).GetLeftPart(UriPartial.Authority);
    public string ServiceKey { get; set; } = default!;
    public string Bucket { get; set; } = "logos";
}
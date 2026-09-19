namespace RiverLine.Api.Settings;

public class ResendSettings
{
    public string ApiKey { get; set; } = default!;
    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = default!;
}
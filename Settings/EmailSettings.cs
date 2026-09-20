namespace RiverLine.Api.Settings;

public class EmailSettings
{
    public string ApiKey { get; set; } = default!;
    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = default!;
}
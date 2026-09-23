namespace RiverLine.Api.Configurations;

public class AppUrls
{
    public string BaseUrl { get; set; } = "";
 
    public string Shipment(Guid id) => $"{BaseUrl.TrimEnd('/')}/shipments/{id}";
    public string Request(Guid id) => $"{BaseUrl.TrimEnd('/')}/requests/{id}";
    public string MyOffers() => $"{BaseUrl.TrimEnd('/')}/offers";
    public string Suggested() => $"{BaseUrl.TrimEnd('/')}/suggested";
    public string Rating() => $"{BaseUrl.TrimEnd('/')}/ratings";
}
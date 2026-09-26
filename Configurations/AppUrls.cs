namespace RiverLine.Api.Configurations;

public class AppUrls
{
    public string BaseUrl { get; set; } = "";
 
    public string Shipment(Guid id) => $"{BaseUrl.TrimEnd('/')}/shipments/{id}";
    public string Request(Guid id) => $"{BaseUrl.TrimEnd('/')}/requests/{id}";
    public string MyOffers() => $"{BaseUrl.TrimEnd('/')}/offers";
    public string Suggested() => $"{BaseUrl.TrimEnd('/')}/suggested";
    public string Rating() => $"{BaseUrl.TrimEnd('/')}/ratings";
    
    
    public string ConfirmEmail(Guid userId, string encodedToken) =>
        $"{BaseUrl.TrimEnd('/')}/auth/confirm-email?userId={userId}&token={encodedToken}";

    public string ResetPassword(string email, string encodedToken) =>
        $"{BaseUrl.TrimEnd('/')}/auth/reset-password?email={Uri.EscapeDataString(email)}&token={encodedToken}";
}
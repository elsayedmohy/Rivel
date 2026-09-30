namespace RiverLine.Api.Services.Storage;

public class SupabaseStorage(
    HttpClient http,
    IOptions<SupabaseStorageSettings> options,
    ILogger<SupabaseStorage> logger) : IFileStorage
{
    private readonly SupabaseStorageSettings settings = options.Value;

    public async Task UploadAsync(string path, Stream content, string contentType, CancellationToken ct)
    {
        using var body = new StreamContent(content);
        body.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        using var request = new HttpRequestMessage(HttpMethod.Post, ObjectPath(path)) { Content = body };
        request.Headers.Add("x-upsert", "false");  

        using var response = await http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Supabase upload failed ({(int)response.StatusCode}): {error}");
        }
    }

    public async Task TryDeleteAsync(string path)
    {
        try
        {
            using var response = await http.DeleteAsync(ObjectPath(path));
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("Supabase delete failed for {Path}: {Status}", path, (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Supabase delete threw for {Path}", path);
        }
    }

    public string? GetPublicUrl(string? path) =>
        path is null
            ? null
            
            : $"{settings.Url}/storage/v1/object/public/{settings.Bucket}/{path}";
    private string ObjectPath(string path) => $"{settings.Url}/storage/v1/object/{settings.Bucket}/{path}";
}
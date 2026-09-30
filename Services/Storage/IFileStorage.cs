namespace RiverLine.Api.Services.Storage;

public interface IFileStorage
{
    Task UploadAsync(string path, Stream content, string contentType, CancellationToken ct);

    Task TryDeleteAsync(string path);

    string? GetPublicUrl(string? path);
}
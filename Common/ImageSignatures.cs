namespace RiverLine.Api.Common;

public sealed record ImageFormat(string ContentType, string Extension);

public static class ImageSignatures
{
    public const int HeaderLength = 12;
    public const long MaxLogoBytes = 2 * 1024 * 1024;

    public static readonly ImageFormat Png  = new("image/png",  ".png");
    public static readonly ImageFormat Jpeg = new("image/jpeg", ".jpg");
    public static readonly ImageFormat Webp = new("image/webp", ".webp");

    private static ReadOnlySpan<byte> PngMagic => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static ImageFormat? Detect(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 8 && header[..8].SequenceEqual(PngMagic))
            return Png;

        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return Jpeg;

        if (header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8))
            return Webp;

        return null;
    }
}
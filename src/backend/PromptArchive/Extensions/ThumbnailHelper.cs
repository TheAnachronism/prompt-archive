using SkiaSharp;

namespace PromptArchive.Extensions;

public static class ThumbnailHelper
{
    private const int ThumbnailSize = 512;

    public static  MemoryStream GenerateThumbnail(Stream imageStream)
    {
        // Reset stream position
        if (imageStream.CanSeek)
            imageStream.Position = 0;

        using var original = SKBitmap.Decode(imageStream);

        // Calculate new dimensions while maintaining aspect ratio
        var width = original.Width;
        var height = original.Height;

        if (width > height)
        {
            height = (int)(height * ((float)ThumbnailSize / width));
            width = ThumbnailSize;
        }
        else
        {
            width = (int)(width * ((float)ThumbnailSize / height));
            height = ThumbnailSize;
        }

        // Create the thumbnail
        using var thumbnail = original.Resize(new SKImageInfo(width, height), new SKSamplingOptions());
        using var image = SKImage.FromBitmap(thumbnail);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        var thumbnailStream = new MemoryStream();
        data.SaveTo(thumbnailStream);
        thumbnailStream.Position = 0;

        return thumbnailStream;
    }

}
using FluentResults;

namespace PromptArchive.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType,
        CancellationToken cancellationToken = default);

    Task<string> UploadThumbnailAsync(Stream fileStream, string originalFileName, string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsyncTask(string imagePath, CancellationToken cancellationToken = default);
    Task DeleteThumbnailAsyncTask(string thumbnailPath, CancellationToken cancellationToken = default);
    string GetImageUrl(string imagePath);
    string GetThumbnailUrl(string imagePath);

    Task<Result<(Stream Stream, string ContentType)>> GetImageStreamAsync(string imagePath,
        CancellationToken ct = default);

    Task<Result<(Stream Stream, string ContentType)>> GetThumbnailStreamAsync(string imagePath,
        CancellationToken cancellationToken = default);
}
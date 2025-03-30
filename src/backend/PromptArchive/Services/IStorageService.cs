using FluentResults;

namespace PromptArchive.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsyncTask(string imagePath, CancellationToken cancellationToken = default);
    string GetImageUrl(string imagePath);

    Task<Result<(Stream Stream, string ContentType)>> GetImageStreamAsync(string imagePath,
        CancellationToken ct = default);
}
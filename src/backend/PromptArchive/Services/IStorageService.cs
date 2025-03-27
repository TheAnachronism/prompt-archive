namespace PromptArchive.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteImageAsyncTask(string imagePath);
    string GetImageUrl(string imagePath);
}
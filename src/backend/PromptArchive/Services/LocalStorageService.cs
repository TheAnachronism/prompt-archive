using Microsoft.Extensions.Options;
using PromptArchive.Configuration;

namespace PromptArchive.Services;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _uploadDirectory;
    private readonly string _baseUrl;

    public LocalStorageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor,
        IOptions<LocalStorageSettings> storageSettings)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
        _uploadDirectory = storageSettings.Value.UploadDirectory ?? "uploads/images";
        _baseUrl = storageSettings.Value.BaseUrl ?? "/images";
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFilename = $"{Guid.NewGuid()}_{fileName}";

        var uploadPath = Path.Combine(_environment.WebRootPath, _uploadDirectory);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, uniqueFilename);
        await using var fileStream2 = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStream2);

        return Path.Combine(_uploadDirectory, uniqueFilename).Replace("\\", "/");
    }

    public Task DeleteImageAsyncTask(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return Task.CompletedTask;

        var fullPath = Path.Combine(_environment.WebRootPath, _uploadDirectory, imagePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public string GetImageUrl(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return string.Empty;
        
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request is null)
            return Path.Combine(_baseUrl, Path.GetFileName(imagePath)).Replace("\\", "/");

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}{_baseUrl}/{Path.GetFileName(imagePath)}";
    }
}
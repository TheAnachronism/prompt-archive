using FastEndpoints;
using FluentResults;
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

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType,
        CancellationToken cancellationToken = default)
    {
        var uniqueFilename = $"{Guid.NewGuid()}_{fileName}";

        var uploadPath = Path.Combine(_environment.WebRootPath, _uploadDirectory);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, uniqueFilename);
        await using var fileStream2 = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStream2, cancellationToken);

        return uniqueFilename;
    }

    public Task DeleteImageAsyncTask(string imagePath, CancellationToken cancellationToken = default)
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
        var endpoint = _httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<EndpointDefinition>();
        return $"/api/v{endpoint!.Version.Current}/prompts/versions/images/{Uri.EscapeDataString(imagePath)}";
    }

    public Task<Result<(Stream Stream, string ContentType)>> GetImageStreamAsync(string imagePath,
        CancellationToken cancellationToken = default)
    {
        var uploadDir = Path.Combine(_environment.WebRootPath, _uploadDirectory);
        var fullPath = Path.Combine(_environment.WebRootPath, uploadDir, imagePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Result<(Stream Stream, string ContentType)>>(Result.Fail("Image not found"));
        }

        // Determine content type based on file extension
        var contentType = GetContentTypeFromFileName(imagePath);
        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

        return Task.FromResult<Result<(Stream Stream, string ContentType)>>((stream, contentType));
    }

    private static string GetContentTypeFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
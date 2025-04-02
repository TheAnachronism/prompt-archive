using FastEndpoints;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.DownloadPromptVersionImage;

public class DownloadPromptVersionImageEndpoint : Endpoint<PromptIdRequest>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public DownloadPromptVersionImageEndpoint(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("prompts/versions/images/{Id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var image = await _dbContext.PromptVersionImages.FindAsync([req.Id], ct);
        if (image is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = await _storageService.GetImageStreamAsync(image.ImagePath, ct);

        if (result.IsFailed)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        HttpContext.Response.Headers.CacheControl = "public, max-age=86400";
        HttpContext.Response.Headers.ContentDisposition
            = $"attachment; filename=\"{Uri.EscapeDataString(image.OriginalFilename)}\"";

        var contentType = GetContentTypeFromFileName(image.OriginalFilename);
        HttpContext.Response.ContentType = contentType;

        await SendStreamAsync(result.Value.Stream, contentType, cancellation: ct);
    }

    private static string GetContentTypeFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
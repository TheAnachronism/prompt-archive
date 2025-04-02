using FastEndpoints;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.GetThumbnail;

public class GetThumbnailEndpoint : Endpoint<PromptIdRequest>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public GetThumbnailEndpoint(ApplicationDbContext context, IStorageService storageService)
    {
        _dbContext = context;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("prompts/versions/thumbnails/{Id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var image = await _dbContext.PromptVersionImages.FindAsync([req.Id], ct);
        if (image is null || string.IsNullOrEmpty(image.ThumbnailPath))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var result = await _storageService.GetThumbnailStreamAsync(image.ThumbnailPath, ct);

        if (result.IsFailed)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Add caching headers for thumbnails (can be cached longer than originals)
        HttpContext.Response.Headers.CacheControl = "public, max-age=604800"; // 7 days

        // Set content type
        var contentType = result.Value.ContentType;
        HttpContext.Response.ContentType = contentType;

        await SendStreamAsync(result.Value.Stream, contentType, cancellation: ct);
    }
}
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
        
        HttpContext.Response.Headers.ContentDisposition
            = $"attachment; filename=\"{Uri.EscapeDataString(image.OriginalFilename)}\"";
        await SendStreamAsync(result.Value.Stream, cancellation: ct);
    }
}
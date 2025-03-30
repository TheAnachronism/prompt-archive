using FastEndpoints;
using PromptArchive.Services;

namespace PromptArchive.Features.Images;

public class GetImageEndpoint : Endpoint<GetImageRequest>
{
    private readonly IStorageService _storageService;

    public GetImageEndpoint(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("images/{ImagePath}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetImageRequest req, CancellationToken ct)
    {
        var result = await _storageService.GetImageStreamAsync(req.ImagePath, ct);
        if (result.IsFailed)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        HttpContext.Response.Headers.CacheControl = "public, max-age=31536000";
        HttpContext.Response.Headers.Expires = DateTime.UtcNow.AddYears(1).ToString("R");

        await SendStreamAsync(result.Value.Stream, result.Value.ContentType, cancellation: ct);
    }
}
using FastEndpoints;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.PromptThumbnail;

public class PromptThumbnailEndpoint : Endpoint<PromptThumbnailRequest>
{
    public override void Configure()
    {
        Put("/prompts/{PromptId:guid}/thumbnail/{ImageId:guid}");
    }

    public override async Task HandleAsync(PromptThumbnailRequest req, CancellationToken ct)
    {
        var result = await new PromptThumbnailCommand(req.ImageId, req.PromptId).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendOkAsync(ct);
    }
}
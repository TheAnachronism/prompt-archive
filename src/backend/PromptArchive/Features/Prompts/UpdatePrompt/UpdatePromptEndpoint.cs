using FastEndpoints;
using PromptArchive.Extensions;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.UpdatePrompt;

public class UpdatePromptEndpoint : Endpoint<UpdatePromptRequest, PromptResponse>
{
    private readonly IStorageService _storageService;

    public UpdatePromptEndpoint(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override void Configure()
    {
        Put("prompts/{Id:guid}");
    }

    public override async Task HandleAsync(UpdatePromptRequest req, CancellationToken ct)
    {
        var result = await new UpdatePromptCommand(req.Id, req.Title, req.Description, req.Tags, req.Models)
            .ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendOkAsync(result.Value.ToResponse(_storageService), ct);
    }
}
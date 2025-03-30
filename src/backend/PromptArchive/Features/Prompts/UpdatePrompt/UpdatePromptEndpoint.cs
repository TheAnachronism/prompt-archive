using FastEndpoints;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.UpdatePrompt;

public class UpdatePromptEndpoint : Endpoint<UpdatePromptRequest, PromptResponse>
{
    public override void Configure()
    {
        Put("prompts/{Id:guid}");
    }

    public override async Task HandleAsync(UpdatePromptRequest req, CancellationToken ct)
    {
        var result = await new UpdatePromptCommand(req.Id, req.Title, req.Description, req.Tags, req.Models)
            .ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendOkAsync(result.Value.ToResponse(), ct);
    }
}
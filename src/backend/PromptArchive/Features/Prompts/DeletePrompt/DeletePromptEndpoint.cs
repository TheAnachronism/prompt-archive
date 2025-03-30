using FastEndpoints;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.DeletePrompt;

public class DeletePromptEndpoint : Endpoint<DeletePromptRequest>
{
    public override void Configure()
    {
        Delete("/prompts/{Id:guid}");
    }

    public override async Task HandleAsync(DeletePromptRequest req, CancellationToken ct)
    {
        var result = await new DeletePromptCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendNoContentAsync(ct);
    }
}
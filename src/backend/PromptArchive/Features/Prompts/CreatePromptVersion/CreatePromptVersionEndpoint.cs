using FastEndpoints;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.GetPrompt;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionEndpoint : Endpoint<CreatePromptVersionRequest, PromptResponse>
{
    public override void Configure()
    {
        Post("prompts/{Id:guid}/versions");
    }

    public override async Task HandleAsync(CreatePromptVersionRequest req, CancellationToken ct)
    {
        var promptResult = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(promptResult);

        var result = await new CreatePromptVersionCommand(promptResult.Value, req.PromptContent).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        promptResult = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(promptResult);

        await SendOkAsync(promptResult.Value.ToResponse(), ct);
    }
}
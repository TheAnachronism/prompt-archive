using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.GetPrompt;

public class GetPromptByIdEndpoint : Endpoint<PromptIdRequest, PromptResponse>
{
    public override void Configure()
    {
        Get("prompts/{Id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var result = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors();
        }

        await SendOkAsync(result.Value.ToResponse(), ct);
    }
}
using FastEndpoints;

namespace PromptArchive.Features.Prompts.GetPrompts;

public class GetPromptsEndpoint : Endpoint<GetPromptsRequest, PromptListResponse>
{
    public override void Configure()
    {
        Get("prompts");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetPromptsRequest req, CancellationToken ct)
    {
        var result = await new GetPromptsCommand(req.Page, req.PageSize, req.SearchTerm, req.UserId).ExecuteAsync(ct);
        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors();
        }

        await SendOkAsync(result.Value, ct);
    }
}
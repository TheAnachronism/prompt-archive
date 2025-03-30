using FastEndpoints;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.GetPromptComments;

public class GetPromptCommentsEndpoint : Endpoint<PromptIdRequest, List<PromptCommentResponse>>
{
    public override void Configure()
    {
        Get("prompts/{Id:guid}/comments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var result = await new GetPromptCommentsCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendOkAsync(result.Value.Select(c => c.ToResponse()).ToList(), ct);
    }
}
using FastEndpoints;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptsEndpoint : Endpoint<GetPromptsRequest, PromptListResponse>
{
    private readonly ILogger<GetPromptsEndpoint> _logger;

    public GetPromptsEndpoint(ILogger<GetPromptsEndpoint> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("prompts");
        Group<PromptEndpoints>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetPromptsRequest req, CancellationToken ct)
    {
        var result = await new GetPromptsCommand(req.Page, req.PageSize, req.SearchTerm, req.Tag, req.UserId)
            .ExecuteAsync(ct);

        if (result.IsSuccess)
        {
            await SendOkAsync(result.Value, ct);
            return;
        }

        _logger.LogWarning("Failed to get prompts: {Errors}", string.Join("\n", result.Errors.Select(e => e.Message)));
    }
}
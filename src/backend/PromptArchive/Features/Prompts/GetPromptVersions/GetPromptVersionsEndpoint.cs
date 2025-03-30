using FastEndpoints;
using PromptArchive.Extensions;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.GetPromptVersions;

public class GetPromptVersionsEndpoint : Endpoint<PromptIdRequest, List<PromptVersionResponse>>
{
    private readonly IStorageService _storageService;

    public GetPromptVersionsEndpoint(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("prompts/{Id}/versions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var versionResult = await new GetPromptVersionsCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(versionResult);

        await SendOkAsync(versionResult.Value.Select(v => v.ToResponse(_storageService)).ToList(), ct);
    }
}
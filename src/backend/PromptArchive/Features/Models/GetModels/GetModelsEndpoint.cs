using FastEndpoints;
using PromptArchive.Extensions;
using PromptArchive.Features.Tags.GetTags;

namespace PromptArchive.Features.Models.GetModels;

public class GetModelsEndpoint : EndpointWithoutRequest<List<ModelResponse>>
{
    public override void Configure()
    {
        Get("models");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tagsResult = await new GetTagsCommand().ExecuteAsync(ct);
        this.ThrowIfAnyErrors(tagsResult);

        await SendOkAsync(tagsResult.Value.Select(t => new ModelResponse
        {
            Id = t.Id,
            Name = t.Name,
            PromptCount = t.PromptTags.Count
        }).ToList(), ct);
    }
}
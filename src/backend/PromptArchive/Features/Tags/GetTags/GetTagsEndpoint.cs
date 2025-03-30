using FastEndpoints;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Tags.GetTags;

public class GetTagsEndpoint : EndpointWithoutRequest<List<TagResponse>>
{
    public override void Configure()
    {
        Get("tags");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tagsResult = await new GetTagsCommand().ExecuteAsync(ct);
        this.ThrowIfAnyErrors(tagsResult);

        await SendOkAsync(tagsResult.Value.Select(t => new TagResponse
        {
            Id = t.Id,
            Name = t.Name,
            PromptCount = t.PromptTags.Count
        }).ToList(), ct);
    }
}
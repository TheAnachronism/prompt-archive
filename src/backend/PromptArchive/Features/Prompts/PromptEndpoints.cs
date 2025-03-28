using FastEndpoints;

namespace PromptArchive.Features.Prompts;

public sealed class PromptEndpoints : Group
{
    public PromptEndpoints()
    {
        Configure("prompts", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("prompts"));
        });
    }
}
using FastEndpoints;
using FluentResults;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptByIdQuery : ICommand<Result<PromptResponse>>
{
    public GetPromptByIdQuery(Guid promptId)
    {
        PromptId = promptId;
    }

    public Guid PromptId { get; }
}
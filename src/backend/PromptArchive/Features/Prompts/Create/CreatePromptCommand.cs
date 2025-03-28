using FastEndpoints;
using FluentResults;
using PromptArchive.Database;
using PromptArchive.Features.Prompts.Get;

namespace PromptArchive.Features.Prompts.Create;

public class CreatePromptCommand : ICommand<Result<PromptResponse>>
{
    public CreatePromptCommand(ApplicationUser user, string title, string description, List<string>? tags)
    {
        User = user;
        Title = title;
        Description = description;
        Tags = tags;
    }

    public ApplicationUser User { get; }
    public string Title { get; }
    public string Description { get; }
    public List<string>? Tags { get; }

}
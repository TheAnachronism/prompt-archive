using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionRequest
{
    public Guid Id { get; set; }
    public string PromptContent { get; set; } = string.Empty;
}

public class CreatePromptVersionRequestValidator : Validator<CreatePromptVersionRequest>
{
    public CreatePromptVersionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();
    }
}
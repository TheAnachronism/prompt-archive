using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionRequest
{
    public Guid PromptId { get; set; }
    public string PromptContent { get; set; } = string.Empty;
}

public class CreatePromptVersionRequestValidator : Validator<CreatePromptVersionRequest>
{
    public CreatePromptVersionRequestValidator()
    {
        RuleFor(x => x.PromptId).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();
    }
}
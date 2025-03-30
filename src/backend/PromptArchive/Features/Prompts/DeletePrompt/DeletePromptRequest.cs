using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.DeletePrompt;

public class DeletePromptRequest
{
    public Guid Id { get; set; }
}

public class DeletePromptRequestValidator : Validator<DeletePromptRequest>
{
    public DeletePromptRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
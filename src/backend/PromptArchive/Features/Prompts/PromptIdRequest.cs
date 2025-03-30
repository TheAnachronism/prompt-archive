using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts;

public class PromptIdRequest
{
    public Guid Id { get; set; }
}

public class PromptByIdRequestValidator : Validator<PromptIdRequest>
{
    public PromptByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

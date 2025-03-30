using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.AddPromptComment;

public class AddPromptCommentRequest
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AddPromptCommentRequestValidator : Validator<AddPromptCommentRequest>
{
    public AddPromptCommentRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}
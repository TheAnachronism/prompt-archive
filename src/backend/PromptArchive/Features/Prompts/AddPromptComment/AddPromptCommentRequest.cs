using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.AddPromptComment;

public class AddPromptCommentRequest
{
    public Guid PromptId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AddPromptCommentRequestValidator : Validator<AddPromptCommentRequest>
{
    public AddPromptCommentRequestValidator()
    {
        RuleFor(x => x.PromptId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}
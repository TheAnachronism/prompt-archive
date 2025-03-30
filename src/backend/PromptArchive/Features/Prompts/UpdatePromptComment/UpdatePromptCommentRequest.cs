using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.UpdatePromptComment;

public class UpdatePromptCommentRequest
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class UpdatePromptCommentRequestValidator : Validator<UpdatePromptCommentRequest>
{
    public UpdatePromptCommentRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}
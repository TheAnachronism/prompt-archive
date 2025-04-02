using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.PromptThumbnail;

public class PromptThumbnailRequest
{
    public Guid ImageId { get; set; }
    public Guid PromptId { get; set; }
}

public class PromptThumbnailRequestValidator : Validator<PromptThumbnailRequest>
{
    public PromptThumbnailRequestValidator()
    {
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.PromptId).NotEmpty();
    }
}
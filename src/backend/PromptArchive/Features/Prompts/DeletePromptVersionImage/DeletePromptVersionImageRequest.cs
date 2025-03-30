using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.DeletePromptVersionImage;

public class DeletePromptVersionImageRequest
{
    public Guid ImageId { get; set; }
}

public class DeletePromptVersionImageRequestValidator : Validator<DeletePromptVersionImageRequest>
{
    public DeletePromptVersionImageRequestValidator()
    {
        RuleFor(x => x.ImageId).NotEmpty();
    }
}
using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Images;

public class GetImageRequest
{
    public string ImagePath { get; set; } = string.Empty;
}

public class GetImageRequestValidator : Validator<GetImageRequest>
{
    public GetImageRequestValidator()
    {
        RuleFor(x => x.ImagePath).NotEmpty();
    }
}
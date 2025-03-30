using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionRequest
{
    public Guid Id { get; set; }
    public string PromptContent { get; set; } = string.Empty;
    public List<IFormFile>? Images { get; set; }
    public Dictionary<string, string>? ImageCaptions { get; set; }
}

public class CreatePromptVersionRequestValidator : Validator<CreatePromptVersionRequest>
{
    private static readonly string[] AllowedTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    public CreatePromptVersionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();

        RuleForEach(x => x.Images)
            .Must(file =>
            {
                if (file is null) return true;

                return file.Length <= 50 * 1024 * 1024 && AllowedTypes.Contains(file.ContentType);
            })
            .WithMessage("Invalid image file. Only JPEG, PNG, GIF and WebP files under 50 MB are allowed.");
    }
}
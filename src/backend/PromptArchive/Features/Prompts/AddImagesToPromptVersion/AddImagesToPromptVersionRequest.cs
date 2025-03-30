using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.AddImagesToPromptVersion;

public class AddImagesToPromptVersionRequest
{
    public Guid VersionId { get; set; }

    public List<IFormFile> Images { get; set; } = new();
    public Dictionary<string, string>? ImageCaptions { get; set; }
}

public class AddImagesToPromptVersionRequestValidator : Validator<AddImagesToPromptVersionRequest>
{
    public AddImagesToPromptVersionRequestValidator()
    {
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image must be provided");

        RuleForEach(x => x.Images)
            .Must(file =>
            {
                if (file == null) return false;

                // Check file size (e.g., 10MB max)
                if (file.Length > 10 * 1024 * 1024) return false;

                // Check file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                return allowedTypes.Contains(file.ContentType);
            })
            .WithMessage("Invalid image file. Only JPEG, PNG, GIF, and WebP files under 10MB are allowed.");
    }
}
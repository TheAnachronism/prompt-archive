using System.Text.Json;
using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.AddImagesToPromptVersion;

public class AddImagesToPromptVersionRequest
{
    public Guid VersionId { get; set; }

    public List<IFormFile> Images { get; set; } = new();
    public string? ImageCaptionsJson { get; set; }

    public Dictionary<string, string>? GetImageCaptions()
    {
        if (string.IsNullOrEmpty(ImageCaptionsJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(ImageCaptionsJson);
        }
        catch
        {
            return null;
        }
    }
}

public class AddImagesToPromptVersionRequestValidator : Validator<AddImagesToPromptVersionRequest>
{
    private static readonly string[] FileFormats = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    public AddImagesToPromptVersionRequestValidator(IConfiguration configuration)
    {
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image must be provided");

        RuleForEach(x => x.Images)
            .Must(file =>
            {
                if (file == null) return false;

                if (file.Length > configuration.GetValue<int>("MaxUploadSize")) return false;

                return FileFormats.Contains(file.ContentType);
            })
            .WithMessage("Invalid image file. Only JPEG, PNG, GIF, and WebP files under 10MB are allowed.");
    }
}
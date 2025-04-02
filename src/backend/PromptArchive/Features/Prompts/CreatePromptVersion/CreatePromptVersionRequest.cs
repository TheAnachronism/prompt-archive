using System.Text.Json;
using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionRequest
{
    public Guid Id { get; set; }
    public string PromptContent { get; set; } = string.Empty;
    public List<IFormFile>? Images { get; set; }
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

public class CreatePromptVersionRequestValidator : Validator<CreatePromptVersionRequest>
{
    private static readonly string[] AllowedTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    public CreatePromptVersionRequestValidator(IConfiguration configuration)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();

        RuleForEach(x => x.Images)
            .Must(file =>
            {
                if (file == null) return false;

                if (file.Length > configuration.GetValue<int>("MaxUploadSize")) return false;

                return AllowedTypes.Contains(file.ContentType);
            })
            .WithMessage("Invalid image file. Only JPEG, PNG, GIF, and WebP files under 10MB are allowed.");
    }
}
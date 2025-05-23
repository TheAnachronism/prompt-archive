using System.Text.Json;
using FastEndpoints;
using FluentResults;
using FluentValidation;

namespace PromptArchive.Features.Prompts.CreatePrompt;

public class CreatePromptRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string PromptContent { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> Models { get; set; } = [];
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

public class CreatePromptRequestValidator : Validator<CreatePromptRequest>
{
    public CreatePromptRequestValidator(IConfiguration configuration)
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();
        RuleFor(x => x.Models).NotEmpty();

        RuleForEach(x => x.Images)
            .Must(file =>
            {
                if (file == null) return false;

                // Check file size (e.g., 10MB max)
                if (file.Length > configuration.GetValue<int>("MaxUploadSize")) return false;

                // Check file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                return allowedTypes.Contains(file.ContentType);
            })
            .WithMessage("Invalid image file. Only JPEG, PNG, GIF, and WebP files under 10MB are allowed.");
    }
}
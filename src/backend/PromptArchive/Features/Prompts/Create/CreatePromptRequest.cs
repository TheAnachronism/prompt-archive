using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.Create;

public record CreatePromptRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string>? Tags { get; set; }
    public List<IFormFile>? Images { get; set; }
    public List<ImageMetadata>? ImageMetaData { get; set; }
}

public class CreatePromptRequestValidator : Validator<CreatePromptRequest>
{
    public CreatePromptRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

public record ImageMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
}
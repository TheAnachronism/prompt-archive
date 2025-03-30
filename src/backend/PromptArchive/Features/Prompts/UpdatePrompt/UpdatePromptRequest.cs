using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Prompts.UpdatePrompt;

public class UpdatePromptRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> Models { get; set; } = [];
}

public class UpdatePromptRequestValidator : Validator<UpdatePromptRequest>
{
    public UpdatePromptRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
    }
}
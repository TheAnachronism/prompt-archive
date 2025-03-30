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
}

public class CreatePromptRequestValidator : Validator<CreatePromptRequest>
{
    public CreatePromptRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.PromptContent).NotEmpty();
        RuleFor(x => x.Models).NotEmpty();
    }
}
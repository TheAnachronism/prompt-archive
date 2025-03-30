using System.ComponentModel;
using FastEndpoints;

namespace PromptArchive.Features.Prompts.GetPrompts;

public class GetPromptsRequest
{
    [QueryParam] [DefaultValue(1)] public int Page { get; set; } = 1;
    [QueryParam] [DefaultValue(10)] public int PageSize { get; set; } = 10;
    [QueryParam] public string? SearchTerm { get; set; }
    [QueryParam] public string? UserId { get; set; }
    [QueryParam] public string[]? Models { get; set; } = [];
    [QueryParam] public string[]? Tags { get; set; } = [];
}
using FastEndpoints;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptsRequest
{
    [QueryParam] public int Page { get; set; }
    [QueryParam] public int PageSize { get; set; }
    [QueryParam] public string? SearchTerm { get; set; }
    [QueryParam] public string? Tag { get; set; }
    [QueryParam] public string? UserId { get; set; }
}
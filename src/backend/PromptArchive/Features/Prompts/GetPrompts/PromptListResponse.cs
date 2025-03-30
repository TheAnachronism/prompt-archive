namespace PromptArchive.Features.Prompts.GetPrompts;

public class PromptListResponse
{
    public List<PromptResponse> Prompts { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
}
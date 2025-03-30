namespace PromptArchive.Features.Tags;

public class TagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PromptCount { get; set; }
}
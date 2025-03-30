namespace PromptArchive.Database;

public class PromptTag
{
    public Guid PromptId { get; set; }
    public Prompt Prompt { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
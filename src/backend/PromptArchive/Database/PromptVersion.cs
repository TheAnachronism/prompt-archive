namespace PromptArchive.Database;

public class PromptVersion
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; }
    public string PromptContent { get; set; } = null!;
    public int VersionNumber { get; set; }

    public ICollection<PromptVersionImage> Images { get; set; } = new List<PromptVersionImage>();

    public Guid PromptId { get; set; }
    public Prompt Prompt { get; set; } = null!;
}
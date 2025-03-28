namespace PromptArchive.Database;

public class PromptVersion
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PromptId { get; set; }
    public Prompt Prompt { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public string VersionHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public ICollection<PromptImage> Images { get; set; } = new List<PromptImage>();
}
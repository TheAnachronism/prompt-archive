namespace PromptArchive.Database;

public class Prompt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public PromptVersionImage? ThumbnailImage { get; set; }

    public ICollection<PromptVersion> PromptVersions { get; set; } = new List<PromptVersion>();
    public ICollection<PromptComment> Comments { get; set; } = new List<PromptComment>();
    public ICollection<PromptModel> PromptModels { get; set; } = new List<PromptModel>();
    public ICollection<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
}
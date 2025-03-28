using System.Collections;

namespace PromptArchive.Database;

public class Prompt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public ICollection<PromptVersion> Versions { get; set; } = new List<PromptVersion>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<PromptComment> Comments { get; set; } = new List<PromptComment>();
}
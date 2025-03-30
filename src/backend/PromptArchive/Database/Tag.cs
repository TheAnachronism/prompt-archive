namespace PromptArchive.Database;

public class Tag
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;

    public ICollection<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
}
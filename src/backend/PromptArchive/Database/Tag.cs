namespace PromptArchive.Database;

public class Tag
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<Prompt> Prompts { get; set; } = new List<Prompt>();
}
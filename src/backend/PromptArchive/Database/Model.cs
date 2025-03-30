namespace PromptArchive.Database;

public class Model
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;

    public ICollection<PromptModel> PromptModels { get; set; } = new List<PromptModel>();
}
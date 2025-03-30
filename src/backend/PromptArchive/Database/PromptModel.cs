namespace PromptArchive.Database;

public class PromptModel
{
    public Guid PromptId { get; set; }
    public Prompt Prompt { get; set; } = null!;

    public Guid ModelId { get; set; }
    public Model Model { get; set; } = null!;
}
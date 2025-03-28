namespace PromptArchive.Database;

public class PromptImage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PromptVersionId { get; set; }
    public PromptVersion PromptVersion { get; set; } = null!;
    public string ImagePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
}
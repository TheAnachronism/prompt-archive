namespace PromptArchive.Database;

public class PromptVersionImage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string ImagePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Caption { get; set; }

    public string OriginalFilename { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public Guid PromptVersionId { get; set; }
    public PromptVersion PromptVersion { get; set; } = null!;
}
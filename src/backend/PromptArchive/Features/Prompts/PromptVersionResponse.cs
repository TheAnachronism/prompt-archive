using PromptArchive.Database;

namespace PromptArchive.Features.Prompts;

public class PromptVersionResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PromptContent { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public Guid PromptId { get; set; }
}

public static class PromptVersionResponseMapper
{
    public static PromptVersionResponse ToResponse(this PromptVersion promptVersion) => new()
    {
        Id = promptVersion.Id,
        CreatedAt = promptVersion.CreatedAt,
        PromptContent = promptVersion.PromptContent,
        PromptId = promptVersion.PromptId,
        VersionNumber = promptVersion.VersionNumber
    };
}
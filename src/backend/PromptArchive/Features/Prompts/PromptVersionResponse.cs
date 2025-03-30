using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts;

public class PromptVersionResponse
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string PromptContent { get; init; } = string.Empty;
    public int VersionNumber { get; init; }
    public Guid PromptId { get; init; }
    public List<PromptVersionImageResponse> Images { get; init; } = new();
}

public class PromptVersionImageResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? Caption { get; init; }
    public DateTime CreatedAt { get; init; }
    public string OriginalFileName { get; set; } = null!;
    public long FileSizeBytes { get; set; }
}

public static class PromptVersionResponseMapper
{
    public static PromptVersionResponse ToResponse(this PromptVersion promptVersion, IStorageService storageService) =>
        new()
        {
            Id = promptVersion.Id,
            CreatedAt = promptVersion.CreatedAt,
            PromptContent = promptVersion.PromptContent,
            PromptId = promptVersion.PromptId,
            VersionNumber = promptVersion.VersionNumber,
            Images = promptVersion.Images.Select(x => x.ToResponse(storageService)).ToList(),
        };

    public static PromptVersionImageResponse ToResponse(this PromptVersionImage promptVersionImage,
        IStorageService storageService) => new()
    {
        Id = promptVersionImage.Id,
        ImageUrl = storageService.GetImageUrl(promptVersionImage.Id.ToString()),
        Caption = promptVersionImage.Caption,
        CreatedAt = promptVersionImage.CreatedAt,
        OriginalFileName = promptVersionImage.OriginalFilename,
        FileSizeBytes = promptVersionImage.FileSizeBytes,
    };
}
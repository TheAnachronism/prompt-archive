using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts;

public class PromptResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public IEnumerable<string> Tags { get; init; } = [];
    public IEnumerable<string> Models { get; set; } = [];
    public int VersionCount { get; init; }
    public PromptVersionResponse LatestVersion { get; init; } = null!;
    public int CommentCount { get; init; }
}

public static class PromptResponseMapper
{
    public static PromptResponse ToResponse(this Prompt prompt, IStorageService storageService) =>
        new()
        {
            Id = prompt.Id,
            Title = prompt.Title,
            Description = prompt.Description,
            CreatedAt = prompt.CreatedAt,
            UpdatedAt = prompt.UpdatedAt,
            UserId = prompt.UserId,
            UserName = prompt.User.UserName!,
            VersionCount = prompt.PromptVersions.Count,
            Tags = prompt.PromptTags.Select(x => x.Tag.Name),
            Models = prompt.PromptModels.Select(m => m.Model.Name),
            LatestVersion = prompt.PromptVersions.OrderByDescending(v => v.VersionNumber).First()
                .ToResponse(storageService),
            CommentCount = prompt.Comments.Count
        };
}
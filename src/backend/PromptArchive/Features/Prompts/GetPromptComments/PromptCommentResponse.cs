using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.GetPromptComments;

public class PromptCommentResponse
{
    public Guid Id { get; set; }
    public Guid PromptId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public static class PromptCommentResponseMapper
{
    public static PromptCommentResponse ToResponse(this PromptComment comment) => new()
    {
        Id = comment.Id,
        PromptId = comment.PromptId,
        Content = comment.Content,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt,
        UserId = comment.UserId,
        UserName = comment.User.UserName!,
    };
}
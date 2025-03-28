namespace PromptArchive.Features.Prompts.Get;

public record PromptListResponse(List<PromptResponse> Prompts, int TotalCount, int PageSize, int CurrentPage);
public record PromptResponse(Guid Id, string Title, string Description, DateTime CreatedAt, DateTime UpdatedAt, string UserId, string UserName, IEnumerable<string> Tags, IEnumerable<PromptVersionResponse> Versions, int VersionCount, int CommentCount);
public record PromptVersionResponse(Guid Id, Guid PromptId, string Content, string VersionHash, DateTime CreatedAt, string UserId, string UserName, IEnumerable<PromptImageResponse> Images);
public record PromptImageResponse(Guid Id, string Url, string FileName, string? Caption, int DisplayOrder);
namespace PromptArchive.Features;

public record UserDto(string Id, string UserName, string Email, List<string> Roles, bool EmailConfirmed, DateTime CreatedAt, DateTime? LastLoginAt);
namespace PromptArchive.Features.Auth;

public record UserResponse(string Id, string Email, string UserName)
{
    public static UserResponse Empty => new UserResponse(string.Empty, string.Empty, string.Empty);
};
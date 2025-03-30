using FastEndpoints;

namespace PromptArchive.Features.UserManagement.ListUsers;

public class ListUsersRequest
{
    [QueryParam]
    public int Page { get; set; } = 1;
    [QueryParam]
    public int PageSize { get; set; } = 10;
    [QueryParam]
    public string? SearchTerm { get; set; }
}
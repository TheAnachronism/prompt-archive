using FastEndpoints;

namespace PromptArchive.Features.UserManagement;

public class IdRequest
{
    [QueryParam]
    public string Id { get; set; } = string.Empty;
}
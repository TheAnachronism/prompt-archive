using FastEndpoints;

namespace PromptArchive.Features.UserManagement.UpdateUser;

public class UpdateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; }
}

public class UpdateUserRequestValidator : Validator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        
    }
}
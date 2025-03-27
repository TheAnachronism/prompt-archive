using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.UserManagement.DeleteUser;

public class DeleteUserEndpoint : Endpoint<IdRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DeleteUserEndpoint> _logger;

    public DeleteUserEndpoint(UserManager<ApplicationUser> userManager, ILogger<DeleteUserEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("manage/users/{Id}");
        Policies("Admin");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (User.FindFirst("sub")?.Value == req.Id)
        {
            AddError("You cannot delete your own account");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);

            await SendErrorsAsync(400, ct);
            return;
        }

        await SendNoContentAsync(cancellation: ct);
    }
}
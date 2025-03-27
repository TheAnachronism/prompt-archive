using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.UserManagement.ChangePassword;

public class ChangePasswordEndpoint : Endpoint<ChangePasswordRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ChangePasswordEndpoint> _logger;

    public ChangePasswordEndpoint(UserManager<ApplicationUser> userManager, ILogger<ChangePasswordEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("users/change-password");
        Policies("Admin");
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.UserId);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, req.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);

            await SendErrorsAsync(400, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
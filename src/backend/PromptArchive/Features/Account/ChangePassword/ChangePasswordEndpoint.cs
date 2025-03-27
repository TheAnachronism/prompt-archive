using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using ILogger = Serilog.ILogger;

namespace PromptArchive.Features.Account.ChangePassword;

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
        Post("account/change-password");
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ThrowError("User not found");
            return;
        }

        var result = await _userManager.ChangePasswordAsync(user, req.Password, req.NewPassword);
        if (!result.Succeeded)
        {
            foreach(var error in result.Errors) AddError(error.Description);
            ThrowIfAnyErrors();
            return;
        }

        await SendNoContentAsync();
    }
}
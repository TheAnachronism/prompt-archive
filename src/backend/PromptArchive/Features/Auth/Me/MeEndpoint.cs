using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.Me;

public class MeEndpoint : EndpointWithoutRequest<UserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public MeEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("auth/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        await SendOkAsync(new UserResponse(user.Id, user.Email!, user.UserName!, roles), cancellation: ct);
    }
}
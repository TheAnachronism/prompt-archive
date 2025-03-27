using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Account.GetProfile;

public class GetProfileEndpoint : EndpointWithoutRequest<UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetProfileEndpoint> _logger;

    public GetProfileEndpoint(UserManager<ApplicationUser> userManager, ILogger<GetProfileEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("account/profile");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ThrowError("User not found");
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);

        await SendOkAsync(new UserDto(user.Id, user.UserName!, user.Email!, roles.ToList(), user.EmailConfirmed,
            user.CreatedAt,
            user.LastLoginAt), cancellation: ct);
    }
}
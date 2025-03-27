using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Features.Auth;

namespace PromptArchive.Features.UserManagement.GetUser;

public class GetUserEndpoint : Endpoint<IdRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ApplicationUser> _logger;

    public GetUserEndpoint(UserManager<ApplicationUser> userManager, ILogger<ApplicationUser> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("users/{Id}");
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

        var roles = await _userManager.GetRolesAsync(user);

        await SendOkAsync(new UserDto(user.Id, user.UserName!, user.Email!, roles.ToList(), user.EmailConfirmed,
            user.CreatedAt, user.LastLoginAt), cancellation: ct);
    }
}
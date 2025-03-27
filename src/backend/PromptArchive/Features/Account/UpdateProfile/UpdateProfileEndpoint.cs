using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Account.UpdateProfile;

public class UpdateProfileEndpoint : Endpoint<UpdateProfileRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateProfileEndpoint> _logger;

    public UpdateProfileEndpoint(UserManager<ApplicationUser> userManager, ILogger<UpdateProfileEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("account/profile");
    }

    public override async Task HandleAsync(UpdateProfileRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ThrowError("User not found");
            return;
        }

        user.UserName = req.UserName;

        if (!string.Equals(user.Email, req.Email))
        {
            user.Email = req.Email;
            user.EmailConfirmed = false;
            _logger.LogInformation("User {Id} has changed their email", user.Id);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);
            ThrowIfAnyErrors();
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);
        await SendOkAsync(new UserDto(user.Id, user.UserName!, user.Email!, roles.ToList(), user.EmailConfirmed,
            user.CreatedAt,
            user.LastLoginAt));
    }
}
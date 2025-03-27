using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.UserManagement.UpdateUser;

public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateUserEndpoint> _logger;

    public UpdateUserEndpoint(UserManager<ApplicationUser> userManager, ILogger<UpdateUserEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("manage/users/{Id}");
        Policies("Admin");
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (user.UserName != req.UserName && await _userManager.FindByNameAsync(req.UserName) is not null)
            AddError(x => x.UserName, "Username is already taken");

        if (user.Email != req.Email && await _userManager.FindByEmailAsync(req.Email) is not null)
            AddError(x => x.Email, "Email is already registered");

        ThrowIfAnyErrors();

        user.UserName = req.UserName;
        user.Email = req.Email;
        user.EmailConfirmed = req.EmailConfirmed;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                AddError(error.Description);
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToRemove = currentRoles.Where(r => !req.Roles.Contains(r)).ToList();
        if (rolesToRemove.Count > 0)
        {
            result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    AddError(error.Description);
                await SendErrorsAsync(cancellation: ct);
                return;
            }
        }

        var rolesToAdd = req.Roles.Where(r => !currentRoles.Contains(r)).ToList();
        if (rolesToAdd.Count != 0)
        {
            result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) AddError(error.Description);

                await SendErrorsAsync(400, ct);
                return;
            }
        }

        var updatedRoles = await _userManager.GetRolesAsync(user);

        await SendAsync(
            new UserDto(user.Id, user.UserName!, user.Email!, updatedRoles.ToList(), user.EmailConfirmed,
                user.CreatedAt,
                user.LastLoginAt), cancellation: ct);
    }
}
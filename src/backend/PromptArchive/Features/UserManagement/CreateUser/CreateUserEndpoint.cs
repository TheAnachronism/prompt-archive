using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using ILogger = Serilog.ILogger;

namespace PromptArchive.Features.UserManagement.CreateUser;

public class CreateUserEndpoint : Endpoint<CreateUserRequest, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateUserEndpoint> _logger;

    public CreateUserEndpoint(UserManager<ApplicationUser> userManager, ILogger<CreateUserEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("manage/users");
        Policies("Admin");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        if (await _userManager.FindByNameAsync(req.UserName) is not null)
        {
            AddError(x => x.UserName, "Username is already taken");
            ThrowIfAnyErrors();
            return;
        }

        if (await _userManager.FindByEmailAsync(req.Email) is not null)
        {
            AddError(x => x.Email, "Email is already registered");
            ThrowIfAnyErrors();
            return;
        }

        var user = new ApplicationUser
        {
            UserName = req.UserName,
            Email = req.Email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);
            _logger.LogWarning("Failed creating a new user: {Errors}",
                string.Join("\n", result.Errors.Select(x => x.Description)));
            ThrowIfAnyErrors();
            return;
        }

        if (req.Roles.Count != 0)
        {
            result = await _userManager.AddToRolesAsync(user, req.Roles);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) AddError(error.Description);
                _logger.LogWarning("Failed adding new user {User} to roles {Roles}: {Errors}",
                    user.UserName, string.Join(",", req.Roles),
                    string.Join("\n", result.Errors.Select(x => x.Description)));
                ThrowIfAnyErrors();
                return;
            }
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        var roles = await _userManager.GetRolesAsync(user);

        await SendOkAsync(
            new UserDto(user.Id, user.UserName!, user.Email!, roles.ToList(), user.EmailConfirmed, user.CreatedAt,
                user.LastLoginAt), cancellation: ct);
    }
}
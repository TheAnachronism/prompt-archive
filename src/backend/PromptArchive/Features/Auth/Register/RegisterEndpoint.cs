using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<RegisterEndpoint> _logger;

    public RegisterEndpoint(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterEndpoint> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _userManager.FindByEmailAsync(req.Email) is not null ||
            await _userManager.FindByNameAsync(req.UserName) is not null)
            AddError(x => x.Email, "User with username or email already exists");

        ThrowIfAnyErrors();

        var user = new ApplicationUser
        {
            UserName = req.UserName,
            Email = req.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);
            ThrowIfAnyErrors();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        await SendAsync(new UserResponse(user.Id, user.Email, user.UserName), cancellation: ct);
    }
}
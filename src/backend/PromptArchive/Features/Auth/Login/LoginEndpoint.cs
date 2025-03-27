using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.Login;

public class LoginEndpoint : Endpoint<LoginRequest, UserResponse>
{
    private const string FailureMessage = "Invalid email or password";

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginEndpoint> _logger;

    public LoginEndpoint(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
        ILogger<LoginEndpoint> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
        {
            ThrowError(FailureMessage);
            return;
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            req.Password,
            req.RememberMe,
            false);
        
        if (!result.Succeeded)
            ThrowError(FailureMessage);

        await SendOkAsync(new UserResponse(user.Id, user.Email!, user.UserName!), ct);
    }
}
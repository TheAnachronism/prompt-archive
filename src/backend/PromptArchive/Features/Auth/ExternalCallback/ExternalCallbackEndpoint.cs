using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.ExternalCallback;

public class ExternalCallbackRequest
{
    public string ReturnUrl { get; set; } = "/";
}

public class ExternalCallbackEndpoint : Endpoint<ExternalCallbackRequest>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExternalCallbackEndpoint(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("auth/external-callback");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExternalCallbackRequest req, CancellationToken ct)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            await SendRedirectAsync("/login?error=ExternalLoginFailed");
            return;
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            await SendRedirectAsync(req.ReturnUrl);
            return;
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            await SendRedirectAsync("/login?error=NoEmailProvided");
            return;
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // Trust the external provider
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                await SendRedirectAsync("/login?error=UserCreationFailed");
                return;
            }
        }

        // Add the external login to the user
        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
        {
            await SendRedirectAsync("/login?error=AddLoginFailed");
            return;
        }

        // Sign in the user
        await _signInManager.SignInAsync(user, isPersistent: true);
        await SendRedirectAsync(req.ReturnUrl);
    }
}
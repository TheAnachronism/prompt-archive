using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.ExternalLogin;

public class ExternalLoginRequest
{
    public string Provider { get; set; } = "Generic";
    public string ReturnUrl { get; set; } = "/";
}

public class ExternalLoginEndpoint : Endpoint<ExternalLoginRequest>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ExternalLoginEndpoint(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("auth/external-login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExternalLoginRequest req, CancellationToken ct)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(req.Provider, $"api/v1/auth/external-callback?returnUrl={req.ReturnUrl}");

        await HttpContext.ChallengeAsync(req.Provider, properties);
    }
}
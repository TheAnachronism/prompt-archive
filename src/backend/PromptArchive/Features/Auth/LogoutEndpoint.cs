using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth;

public class LogoutEndpoint : EndpointWithoutRequest
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutEndpoint> _logger;

    public LogoutEndpoint(SignInManager<ApplicationUser> signInManager, ILogger<LogoutEndpoint> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Post("auth/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        await SendOkAsync(ct);
    }
}
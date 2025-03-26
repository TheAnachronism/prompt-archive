using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PromptArchive.Database;

namespace PromptArchive.Features.Auth.ForgotPassword;

public class ForgotPasswordEndpoint : Endpoint<ForgotPasswordRequest>
{
    private readonly ILogger<ForgotPasswordEndpoint> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ForgotPasswordEndpoint(ILogger<ForgotPasswordEndpoint> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("auth/forgot-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
        {
            await SendOkAsync(ct);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        _logger.LogInformation("Password reset token for {Email}: {EncodedToken}", user.Email, encodedToken);

        await SendOkAsync(ct);
    }
}
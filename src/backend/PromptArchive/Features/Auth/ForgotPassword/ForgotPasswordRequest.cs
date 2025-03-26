using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Auth.ForgotPassword;

public record ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
};

public class ForgotPasswordRequestValidator : Validator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
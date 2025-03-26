using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.Auth.Register;

public record RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PasswordConfirm { get; init; } = string.Empty;
}

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.PasswordConfirm).Equal(x => x.Password);
    }
}
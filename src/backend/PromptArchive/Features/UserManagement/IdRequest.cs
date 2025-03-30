using FastEndpoints;
using FluentValidation;

namespace PromptArchive.Features.UserManagement;

public class IdRequest
{
    public string Id { get; set; } = string.Empty;
}

public class IdRequestValidator : Validator<IdRequest>
{
    public IdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
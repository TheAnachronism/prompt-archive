using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.DeletePromptVersion;

public class DeletePromptVersionEndpoint : Endpoint<PromptIdRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeletePromptVersionEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("prompts/versions/{id}");
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User is not authenticated");

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var result = await new DeletePromptVersionCommand(req.Id, user.Id, isAdmin).ExecuteAsync(ct: ct);
        this.ThrowIfAnyErrors(result);

        await SendNoContentAsync(ct);
    }
}
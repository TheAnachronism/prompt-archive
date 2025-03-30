using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.DeletePromptComment;

public class DeletePromptCommentEndpoint : Endpoint<PromptIdRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeletePromptCommentEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("comments/{Id:guid}");
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User not found");

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var deleteResult = await new DeletePromptCommentCommand(req.Id, user.Id, isAdmin).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(deleteResult);

        await SendNoContentAsync(ct);
    }
}
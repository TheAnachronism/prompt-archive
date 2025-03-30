using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;

namespace PromptArchive.Features.Prompts.DeletePromptVersionImage;

public class DeletePromptVersionImageEndpoint : Endpoint<DeletePromptVersionImageRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeletePromptVersionImageEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("prompts/versions/images/{ImageId:guid}");
    }

    public override async Task HandleAsync(DeletePromptVersionImageRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User not found");

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var result = await new DeletePromptVersionImageCommand(req.ImageId, user.Id, isAdmin).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendNoContentAsync(ct);
    }
}
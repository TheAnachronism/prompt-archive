using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.GetPromptComments;

namespace PromptArchive.Features.Prompts.UpdatePromptComment;

public class UpdatePromptCommentEndpoint : Endpoint<UpdatePromptCommentRequest, PromptCommentResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdatePromptCommentEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Put("/comments/{Id:guid}");
    }

    public override async Task HandleAsync(UpdatePromptCommentRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User not found");

        var result = await new UpdatePromptCommentCommand(req.Id, req.Content, user.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendOkAsync(result.Value.ToResponse(), ct);
    }
}
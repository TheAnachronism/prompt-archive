using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.GetPrompt;
using PromptArchive.Features.Prompts.GetPromptComments;

namespace PromptArchive.Features.Prompts.AddPromptComment;

public class AddPromptCommentEndpoint : Endpoint<AddPromptCommentRequest, PromptCommentResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddPromptCommentEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("prompts/{PromptId:guid}/comments");
    }

    public override async Task HandleAsync(AddPromptCommentRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User not found");

        var result = await new AddPromptCommentCommand(req.PromptId, user, req.Content).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        await SendCreatedAtAsync<GetPromptByIdEndpoint>(new { Id = result.Value.PromptId }, result.Value.ToResponse(),
            cancellation: ct);
    }
}
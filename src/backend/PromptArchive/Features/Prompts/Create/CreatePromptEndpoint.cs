using Amazon.S3.Model.Internal.MarshallTransformations;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.Create;

public class CreatePromptEndpoint : Endpoint<CreatePromptRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreatePromptEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("prompts");
    }

    public override async Task HandleAsync(CreatePromptRequest req, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ThrowError("User not found");
            return;
        }

        var result = await new CreatePromptCommand(user, req.Title, req.Description, req.Tags).ExecuteAsync(ct);
    }
}
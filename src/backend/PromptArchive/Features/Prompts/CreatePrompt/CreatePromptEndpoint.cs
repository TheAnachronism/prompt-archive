using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Features.Prompts.GetPrompt;

namespace PromptArchive.Features.Prompts.CreatePrompt;

public class CreatePromptEndpoint : Endpoint<CreatePromptRequest, PromptResponse>
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
            AddError("User not found");
            ThrowIfAnyErrors();
            return;
        }

        var result = await req.ToCommand(user).ExecuteAsync(ct);
        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors();
        }
        else
            await SendCreatedAtAsync<GetPromptByIdEndpoint>(new { result.Value.Id }, result.Value.ToResponse(),
                cancellation: ct);
    }
}
using FastEndpoints;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.CreatePromptVersion;

namespace PromptArchive.Features.Prompts.AddImagesToPromptVersion;

public class AddImagesToPromptVersionEndpoint : Endpoint<AddImagesToPromptVersionRequest, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddImagesToPromptVersionEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("prompts/versions/{VersionId:guid}/images");
        AllowFileUploads();
    }

    public override async Task HandleAsync(AddImagesToPromptVersionRequest req, CancellationToken ct)
    {
        var imageUploads = new List<PromptVersionImageUpload>();

        if (req.Images?.Count > 0)
        {
            imageUploads.AddRange(req.Images.Where(image => image.Length > 0)
                .Select(image => new PromptVersionImageUpload
                {
                    ImageStream = image.OpenReadStream(),
                    FileName = image.FileName,
                    ContentType = image.ContentType,
                    Caption = req.ImageCaptions?.GetValueOrDefault(image.FileName),
                    FileSize = image.Length
                }));
        }

        if (imageUploads.Count == 0)
        {
            AddError("At least one valid image must be provided");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            ThrowError("User is not authenticated");

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var result = await new AddImagesToPromptVersionCommand(
            req.VersionId,
            user.Id,
            isAdmin,
            imageUploads
        ).ExecuteAsync(ct);

        this.ThrowIfAnyErrors(result);

        await SendNoContentAsync(ct);
    }
}
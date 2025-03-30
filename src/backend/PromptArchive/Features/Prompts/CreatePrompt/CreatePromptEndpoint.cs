using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using PromptArchive.Database;
using PromptArchive.Features.Prompts.CreatePromptVersion;
using PromptArchive.Features.Prompts.GetPrompt;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.CreatePrompt;

public class CreatePromptEndpoint : Endpoint<CreatePromptRequest, PromptResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStorageService _storageService;

    public CreatePromptEndpoint(UserManager<ApplicationUser> userManager, IStorageService storageService)
    {
        _userManager = userManager;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Post("prompts");
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreatePromptRequest req, CancellationToken ct)
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
                    Caption = req.GetImageCaptions()?.GetValueOrDefault(image.FileName),
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
        {
            AddError("User not found");
            ThrowIfAnyErrors();
            return;
        }

        var result = await req.ToCommand(user, imageUploads).ExecuteAsync(ct);
        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors();
        }
        else
            await SendCreatedAtAsync<GetPromptByIdEndpoint>(new { result.Value.Id },
                result.Value.ToResponse(_storageService),
                cancellation: ct);
    }
}
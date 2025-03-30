using FastEndpoints;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.GetPrompt;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public class CreatePromptVersionEndpoint : Endpoint<CreatePromptVersionRequest, PromptResponse>
{
    private readonly IStorageService _storageService;

    public CreatePromptVersionEndpoint(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override void Configure()
    {
        Post("prompts/{Id:guid}/versions");
        AllowFileUploads();
    }

    public override async Task HandleAsync(CreatePromptVersionRequest req, CancellationToken ct)
    {
        var promptResult = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(promptResult);

        var imageUploads = new List<PromptVersionImageUpload>();

        if (req.Images?.Count > 0)
        {
            imageUploads.AddRange(req.Images.Select(i => new PromptVersionImageUpload
            {
                ImageStream = i.OpenReadStream(),
                FileName = i.FileName,
                ContentType = i.ContentType,
                Caption = req.ImageCaptions?.GetValueOrDefault(i.FileName),
                FileSize = i.Length
            }));
        }

        var result = await new CreatePromptVersionCommand(promptResult.Value, req.PromptContent, imageUploads)
            .ExecuteAsync(ct);
        this.ThrowIfAnyErrors(result);

        promptResult = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        this.ThrowIfAnyErrors(promptResult);

        await SendOkAsync(promptResult.Value.ToResponse(_storageService), ct);
    }
}
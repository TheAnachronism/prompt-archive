using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Features.Prompts.CreatePromptVersion;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.AddImagesToPromptVersion;

public record AddImagesToPromptVersionCommand(
    Guid PromptVersionId,
    string UserId,
    bool IsAdmin,
    List<PromptVersionImageUpload> Images) : ICommand<Result>;

public class AddImagesToPromptVersionCommandHandler : ICommandHandler<AddImagesToPromptVersionCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public AddImagesToPromptVersionCommandHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result> ExecuteAsync(AddImagesToPromptVersionCommand command, CancellationToken ct)
    {
        var version = await _dbContext.PromptVersions.FindAsync([command.PromptVersionId], ct);
        if (version is null)
            return Result.Fail("Prompt version not found");

        version = await _dbContext.PromptVersions
            .Include(v => v.Prompt)
            .FirstAsync(v => v.Id == command.PromptVersionId, ct);

        if (version.Prompt.UserId != command.UserId && !command.IsAdmin)
            return Result.Fail("User is not authorized to manipulate this prompt version");

        foreach (var imageUpload in command.Images)
        {
            try
            {
                var imagePath = await _storageService.UploadImageAsync(imageUpload.ImageStream, imageUpload.FileName,
                    imageUpload.ContentType, ct);

                var image = new PromptVersionImage
                {
                    ImagePath = imagePath,
                    ContentType = imageUpload.ContentType,
                    Caption = imageUpload.Caption,
                    CreatedAt = DateTime.UtcNow,
                    PromptVersion = version,
                    FileSizeBytes = imageUpload.FileSize,
                    OriginalFilename = imageUpload.FileName
                };

                await _dbContext.PromptVersionImages.AddAsync(image, ct);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
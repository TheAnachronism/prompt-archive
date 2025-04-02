using FastEndpoints;
using FluentResults;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public record CreatePromptVersionCommand(
    Prompt Prompt,
    string PromptContent,
    List<PromptVersionImageUpload>? Images = null) : ICommand<Result>;

public class PromptVersionImageUpload
{
    public Stream ImageStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string? Caption { get; set; }
    public long FileSize { get; set; }
}

public class CreatePromptVersionCommandHandler : ICommandHandler<CreatePromptVersionCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly ILogger<CreatePromptVersionCommandHandler> _logger;

    public CreatePromptVersionCommandHandler(ApplicationDbContext dbContext, IStorageService storageService, ILogger<CreatePromptVersionCommandHandler> logger)
    {
        _dbContext = dbContext;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(CreatePromptVersionCommand command, CancellationToken ct)
    {
        var nextVersionNumber = command.Prompt.PromptVersions.Count != 0
            ? command.Prompt.PromptVersions.Max(v => v.VersionNumber) + 1
            : 1;

        var version = new PromptVersion
        {
            Prompt = command.Prompt,
            PromptId = command.Prompt.Id,
            PromptContent = command.PromptContent,
            CreatedAt = DateTime.UtcNow,
            VersionNumber = nextVersionNumber
        };

        await _dbContext.PromptVersions.AddAsync(version, ct);

        if (command.Images?.Count > 0)
        {
            foreach (var imageUpload in command.Images)
            {
                try
                {
                    var imageStreamCopy = new MemoryStream();
                    await imageUpload.ImageStream.CopyToAsync(imageStreamCopy, ct);

                    imageUpload.ImageStream.Position = 0;
                    imageStreamCopy.Position = 0;

                    var imagePath = await _storageService.UploadImageAsync(
                        imageUpload.ImageStream,
                        imageUpload.FileName,
                        imageUpload.ContentType,
                        ct);

                    var thumbnailPath = await _storageService.UploadThumbnailAsync(
                        imageStreamCopy,
                        imageUpload.FileName,
                        imageUpload.ContentType,
                        ct);

                    var image = new PromptVersionImage
                    {
                        ImagePath = imagePath,
                        ThumbnailPath = thumbnailPath,
                        ContentType = imageUpload.ContentType,
                        Caption = imageUpload.Caption,
                        CreatedAt = DateTime.UtcNow,
                        PromptVersion = version,
                        OriginalFilename = imageUpload.FileName,
                        FileSizeBytes = imageUpload.FileSize
                    };

                    await _dbContext.PromptVersionImages.AddAsync(image, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create prompt version image: {FileName}.", imageUpload.FileName);
                    return Result.Fail(ex.Message);
                }
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
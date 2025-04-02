using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.DeletePromptVersionImage;

public record DeletePromptVersionImageCommand(Guid ImageId, string UserId, bool IsAdmin) : ICommand<Result>;

public class DeletePromptVersionImageCommandHandler : ICommandHandler<DeletePromptVersionImageCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public DeletePromptVersionImageCommandHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result> ExecuteAsync(DeletePromptVersionImageCommand command, CancellationToken ct)
    {
        var image = await _dbContext.PromptVersionImages.FindAsync([command.ImageId], ct);
        if (image is null)
            return Result.Fail("Image not found");

        image = await _dbContext.PromptVersionImages
            .Include(i => i.PromptVersion)
            .ThenInclude(v => v.Prompt)
            .FirstAsync(i => i.Id == command.ImageId, ct);

        if (image.PromptVersion.Prompt.UserId != command.UserId && !command.IsAdmin)
            return Result.Fail("User is not authorized to delete this image");

        await _storageService.DeleteImageAsyncTask(image.ImagePath, ct);
        if(!string.IsNullOrEmpty(image.ThumbnailPath))
            await _storageService.DeleteThumbnailAsyncTask(image.ThumbnailPath, ct);

        _dbContext.PromptVersionImages.Remove(image);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
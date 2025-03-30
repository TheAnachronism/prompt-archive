using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.DeletePromptVersion;

public record DeletePromptVersionCommand(Guid Id, string UserId, bool IsAdmin) : ICommand<Result>;

public class DeletePromptVersionCommandHandler : ICommandHandler<DeletePromptVersionCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public DeletePromptVersionCommandHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result> ExecuteAsync(DeletePromptVersionCommand command, CancellationToken ct)
    {
        var version = await _dbContext.PromptVersions.FindAsync([command.Id], ct);
        if (version is null)
            return Result.Fail("Prompt version not found");
        
        if(version.VersionNumber <= 1)
            return Result.Fail("Cannot delete the very first prompt version");

        version = await _dbContext.PromptVersions
            .Include(v => v.Images)
            .Include(v => v.Prompt)
            .FirstAsync(v => v.Id == command.Id, ct);

        if (version.Prompt.UserId != command.UserId && !command.IsAdmin)
            return Result.Fail("You cannot delete this prompt version");

        var errors = new List<Error>();
        foreach (var image in version.Images)
        {
            try
            {
                await _storageService.DeleteImageAsyncTask(image.ImagePath, ct);
                _dbContext.PromptVersionImages.Remove(image);
            }
            catch (Exception ex)
            {
                errors.Add(new Error(ex.Message));
            }
        }

        if (errors.Count > 0)
            return Result.Fail(errors);

        _dbContext.PromptVersions.Remove(version);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
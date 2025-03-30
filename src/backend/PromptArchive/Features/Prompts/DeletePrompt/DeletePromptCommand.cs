using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.DeletePrompt;

public record DeletePromptCommand(Guid Id) : ICommand<Result>;

public class DeletePromptCommandHandler : ICommandHandler<DeletePromptCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public DeletePromptCommandHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result> ExecuteAsync(DeletePromptCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.Id], ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        prompt = await _dbContext.Prompts
            .Include(p => p.PromptVersions)
            .ThenInclude(v => v.Images)
            .Include(p => p.PromptTags)
            .Include(p => p.PromptModels)
            .FirstAsync(p => p.Id == command.Id, ct);

        var errors = new List<Error>();
        foreach (var image in prompt.PromptVersions.SelectMany(v => v.Images))
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

        _dbContext.Prompts.Remove(prompt);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
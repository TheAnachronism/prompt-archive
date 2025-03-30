using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.DeletePrompt;

public record DeletePromptCommand(Guid Id) : ICommand<Result>;

public class DeletePromptCommandHandler : ICommandHandler<DeletePromptCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public DeletePromptCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(DeletePromptCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.Id], ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        prompt = await _dbContext.Prompts
            .Include(p => p.PromptVersions)
            .Include(p => p.PromptTags)
            .Include(p => p.PromptModels)
            .FirstAsync(p => p.Id == command.Id, ct);

        _dbContext.Prompts.Remove(prompt);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
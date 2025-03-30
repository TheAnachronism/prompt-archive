using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.GetPrompt;

public record GetPromptByIdCommand(Guid PromptId) : ICommand<Result<Prompt>>;

public class GetPromptByIdCommandHandler : ICommandHandler<GetPromptByIdCommand, Result<Prompt>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetPromptByIdCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Prompt>> ExecuteAsync(GetPromptByIdCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.PromptId], cancellationToken: ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        prompt = await _dbContext.Prompts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.PromptTags)
            .ThenInclude(t => t.Tag)
            .Include(p => p.PromptModels)
            .ThenInclude(m => m.Model)
            .Include(p => p.PromptVersions)
            .FirstAsync(p => p.Id == command.PromptId, ct);

        return prompt;
    }
}
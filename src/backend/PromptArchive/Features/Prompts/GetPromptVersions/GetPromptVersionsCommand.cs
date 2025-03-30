using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.GetPromptVersions;

public record GetPromptVersionsCommand(Guid PromptId) : ICommand<Result<List<PromptVersion>>>;

public record GetPromptVersionsCommandHandler : ICommandHandler<GetPromptVersionsCommand, Result<List<PromptVersion>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetPromptVersionsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<PromptVersion>>> ExecuteAsync(GetPromptVersionsCommand command, CancellationToken ct)
    {
        return await _dbContext.PromptVersions
            .Include(v => v.Images)
            .Where(v => v.PromptId == command.PromptId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(cancellationToken: ct);
    }
}
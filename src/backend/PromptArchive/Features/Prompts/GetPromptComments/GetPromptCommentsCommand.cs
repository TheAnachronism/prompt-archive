using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.GetPromptComments;

public record GetPromptCommentsCommand(Guid PromptId) : ICommand<Result<List<PromptComment>>>;

public class GetPromptCommentsCommandHandler : ICommandHandler<GetPromptCommentsCommand, Result<List<PromptComment>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetPromptCommentsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<PromptComment>>> ExecuteAsync(GetPromptCommentsCommand command, CancellationToken ct)
    {
        return await _dbContext.PromptComments
            .Include(c => c.User)
            .Where(c => c.PromptId == command.PromptId)
            .ToListAsync(cancellationToken: ct);
    }
}
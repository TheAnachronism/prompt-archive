using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Tags.GetTags;

public record GetTagsCommand() : ICommand<Result<List<Tag>>>;

public class GetTagsCommandHandler : ICommandHandler<GetTagsCommand, Result<List<Tag>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTagsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<Tag>>> ExecuteAsync(GetTagsCommand command, CancellationToken ct)
    {
        return await _dbContext.Tags
            .Include(t => t.PromptTags)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken: ct);
    }
}
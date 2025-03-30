using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Features.Tags.GetTags;

namespace PromptArchive.Features.Models.GetModels;

public record GetModelsCommand() : ICommand<Result<List<Tag>>>;

public class GetModelsCommandHandler : ICommandHandler<GetTagsCommand, Result<List<Tag>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetModelsCommandHandler(ApplicationDbContext dbContext)
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
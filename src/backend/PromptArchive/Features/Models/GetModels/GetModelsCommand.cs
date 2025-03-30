using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Features.Tags.GetTags;

namespace PromptArchive.Features.Models.GetModels;

public record GetModelsCommand() : ICommand<Result<List<Model>>>;

public class GetModelsCommandHandler : ICommandHandler<GetModelsCommand, Result<List<Model>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetModelsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<Model>>> ExecuteAsync(GetModelsCommand command, CancellationToken ct)
    {
        return await _dbContext.Models
            .Include(t => t.PromptModels)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken: ct);
    }
}
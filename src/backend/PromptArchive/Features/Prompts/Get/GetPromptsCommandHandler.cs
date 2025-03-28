using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptsCommandHandler : ICommandHandler<GetPromptsCommand, Result<PromptListResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public GetPromptsCommandHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result<PromptListResponse>> ExecuteAsync(GetPromptsCommand command, CancellationToken ct)
    {
        var query = _dbContext.Prompts
            .Include(p => p.User)
            .Include(p => p.Tags)
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt))
            .ThenInclude(v => v.Images)
            .Include(p => p.Comments)
            .AsQueryable();

        // Filters

        var totalCount = await query.CountAsync(cancellationToken: ct);

        var prompts = await
            query.OrderByDescending(p => p.UpdatedAt)
                .Skip((command.Page - 1) * command.PageSize)
                .Take(command.PageSize)
                .ToListAsync(cancellationToken: ct);

        return new PromptListResponse(prompts.Select(p => p.ToResponse(_storageService)).ToList(), totalCount,
            command.PageSize,
            command.Page);
    }
}
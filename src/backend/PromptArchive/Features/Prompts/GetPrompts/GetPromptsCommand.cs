using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.GetPrompts;

public record GetPromptsCommand(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? UserId,
    string[]? Models,
    string[]? Tags)
    : ICommand<Result<PromptListResponse>>;

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
            .Include(p => p.Comments)
            .Include(p => p.PromptModels)
            .ThenInclude(m => m.Model)
            .Include(p => p.PromptTags)
            .ThenInclude(t => t.Tag)
            .Include(p => p.PromptVersions.OrderByDescending(v => v.VersionNumber))
            .ThenInclude(v => v.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(command.SearchTerm))
        {
            var searchTerm = command.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                p.PromptVersions.Any(v => v.PromptContent.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(command.UserId)) query = query.Where(p => p.UserId == command.UserId);

        if (command.Models?.Length > 0)
            query = query.Where(p => p.PromptModels.Any(pm => command.Models.Any(m => m.ToLower() == pm.Model.NormalizedName)));

        if (command.Tags?.Length > 0)
            query = query.Where(p => command.Tags.All(t => p.PromptTags.Any(pt => pt.Tag.NormalizedName == t.ToLower())));

        var totalCount = await query.CountAsync(ct);

        var prompts = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Skip((command.Page - 1) * command.PageSize)
            .Take(command.PageSize)
            .ToListAsync(cancellationToken: ct);

        var responses = prompts.Select(p => p.ToResponse(_storageService));

        return new PromptListResponse
        {
            Prompts = responses.ToList(),
            TotalCount = totalCount,
            PageSize = command.PageSize,
            CurrentPage = command.Page
        };
    }
}
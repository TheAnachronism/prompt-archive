using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.GetPrompts;

public record GetPromptsCommand(int Page, int PageSize, string? SearchTerm, string? UserId)
    : ICommand<Result<PromptListResponse>>;

public class GetPromptsCommandHandler : ICommandHandler<GetPromptsCommand, Result<PromptListResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetPromptsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var totalCount = await query.CountAsync(ct);

        var prompts = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Skip((command.Page - 1) * command.PageSize)
            .Take(command.PageSize)
            .ToListAsync(cancellationToken: ct);

        var responses = prompts.Select(p => p.ToResponse());

        return new PromptListResponse
        {
            Prompts = responses.ToList(),
            TotalCount = totalCount,
            PageSize = command.PageSize,
            CurrentPage = command.Page
        };
    }
}
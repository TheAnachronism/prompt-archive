using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptByIdQueryHandler : ICommandHandler<GetPromptByIdQuery, Result<PromptResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IStorageService _storageService;

    public GetPromptByIdQueryHandler(ApplicationDbContext dbContext, IStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result<PromptResponse>> ExecuteAsync(GetPromptByIdQuery command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts
            .Include(p => p.User)
            .Include(p => p.Tags)
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt))
            .ThenInclude(v => v.Images)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == command.PromptId, ct);

        if (prompt is null)
            return Result.Fail<PromptResponse>("Prompt not found");

        return new PromptResponse(prompt.Id, prompt.Title, prompt.Description, prompt.CreatedAt, prompt.UpdatedAt,
            prompt.UserId, prompt.User.UserName!, prompt.Tags.Select(x => x.NormalizedName),
            MapToPromptVersionResponses(prompt.Versions),
            prompt.Versions.Count, prompt.Comments.Count);
    }

    private IEnumerable<PromptVersionResponse> MapToPromptVersionResponses(IEnumerable<PromptVersion> versions)
    {
        return versions.Select(v => new PromptVersionResponse(v.Id, v.PromptId, v.Content, v.VersionHash, v.CreatedAt,
            v.UserId, v.User.UserName!, v.Images.Select(i =>
                new PromptImageResponse(i.Id, _storageService.GetImageUrl(i.ImagePath), i.FileName, i.Caption,
                    i.DisplayOrder))));
    }
}
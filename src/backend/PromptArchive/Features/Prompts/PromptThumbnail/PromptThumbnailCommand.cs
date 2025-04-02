using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.PromptThumbnail;

public record PromptThumbnailCommand(Guid ImageId, Guid PromptId) : ICommand<Result>;

public class PromptThumbnailCommandHandler : ICommandHandler<PromptThumbnailCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public PromptThumbnailCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(PromptThumbnailCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.PromptId], ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        var image = await _dbContext.PromptVersionImages.FindAsync([command.ImageId], ct);
        if (image is null)
            return Result.Fail("Image not found");

        prompt.ThumbnailImage = image;
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.GetPrompt;

namespace PromptArchive.Features.Prompts.UpdatePrompt;

public record UpdatePromptCommand(Guid Id, string Title, string? Description, List<string> Tags, List<string> Models)
    : ICommand<Result<Prompt>>;

public class UpdatePromptCommandHandler : ICommandHandler<UpdatePromptCommand, Result<Prompt>>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdatePromptCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Prompt>> ExecuteAsync(UpdatePromptCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.Id], ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        prompt = await _dbContext.Prompts
            .Include(p => p.PromptTags)
            .Include(p => p.PromptModels)
            .FirstAsync(p => p.Id == command.Id, ct);

        prompt.Title = command.Title;
        prompt.Description = command.Description;
        prompt.UpdatedAt = DateTime.UtcNow;

        _dbContext.PromptTags.RemoveRange(prompt.PromptTags);
        _dbContext.PromptModels.RemoveRange(prompt.PromptModels);

        var tags = await TagAndModelHelper.GetAndEnsureTags(_dbContext, command.Tags, ct).ToListAsync(ct);
        prompt.PromptTags = tags.Select(t => new PromptTag
        {
            Prompt = prompt,
            Tag = t
        }).ToList();

        var models = await TagAndModelHelper.GetAndEnsureModels(_dbContext, command.Models, ct).ToListAsync(ct);
        prompt.PromptModels = models.Select(m => new PromptModel
        {
            Prompt = prompt,
            Model = m
        }).ToList();

        await _dbContext.SaveChangesAsync(ct);

        return await new GetPromptByIdCommand(prompt.Id).ExecuteAsync(ct);
    }
}
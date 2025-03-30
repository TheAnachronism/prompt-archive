using System.Runtime.CompilerServices;
using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Extensions;
using PromptArchive.Features.Prompts.CreatePromptVersion;
using PromptArchive.Features.Prompts.GetPrompt;

namespace PromptArchive.Features.Prompts.CreatePrompt;

public class CreatePromptCommand : ICommand<Result<Prompt>>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public string PromptContent { get; init; } = string.Empty;
    public ApplicationUser User { get; init; } = null!;
    public List<string> Models { get; init; } = [];
    public List<string> Tags { get; init; } = [];
}

public static class CreatePromptCommandMapper
{
    public static CreatePromptCommand ToCommand(this CreatePromptRequest request, ApplicationUser user) =>
        new()
        {
            Title = request.Title,
            Description = request.Description,
            PromptContent = request.PromptContent,
            User = user,
            Tags = request.Tags,
            Models = request.Models,
        };
}

public class CreatePromptCommandHandler : ICommandHandler<CreatePromptCommand, Result<Prompt>>
{
    private readonly ApplicationDbContext _dbContext;

    public CreatePromptCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Prompt>> ExecuteAsync(CreatePromptCommand command, CancellationToken ct)
    {
        var prompt = new Prompt
        {
            Title = command.Title,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = command.User,
            UserId = command.User.Id
        };

        var tags = await TagAndModelHelper.GetAndEnsureTags(_dbContext, command.Tags, ct).ToListAsync(ct);
        prompt.PromptTags = tags.Select(t => new PromptTag
        {
            Prompt = prompt,
            Tag = t,
        }).ToList();

        var models = await TagAndModelHelper.GetAndEnsureModels(_dbContext, command.Models, ct).ToListAsync(ct);
        prompt.PromptModels = models.Select(m => new PromptModel
        {
            Prompt = prompt,
            Model = m,
        }).ToList();

        await _dbContext.SaveChangesAsync(ct);

        var versionResult = await new CreatePromptVersionCommand(prompt, command.PromptContent).ExecuteAsync();

        if (versionResult.IsFailed)
            return Result.Fail(versionResult.Errors);

        return await new GetPromptByIdCommand(prompt.Id).ExecuteAsync(ct);
    }
}
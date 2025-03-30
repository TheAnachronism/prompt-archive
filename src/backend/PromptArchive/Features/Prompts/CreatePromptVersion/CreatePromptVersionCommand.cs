using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.CreatePromptVersion;

public record CreatePromptVersionCommand(Prompt Prompt, string PromptContent) : ICommand<Result>;

public class CreatePromptVersionCommandHandler : ICommandHandler<CreatePromptVersionCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public CreatePromptVersionCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(CreatePromptVersionCommand command, CancellationToken ct)
    {
        var nextVersionNumber = command.Prompt.PromptVersions.Any()
            ? command.Prompt.PromptVersions.Max(v => v.VersionNumber) + 1
            : 1;

        var version = new PromptVersion
        {
            Prompt = command.Prompt,
            PromptId = command.Prompt.Id,
            PromptContent = command.PromptContent,
            CreatedAt = DateTime.UtcNow,
            VersionNumber = nextVersionNumber
        };

        _dbContext.PromptVersions.Add(version);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
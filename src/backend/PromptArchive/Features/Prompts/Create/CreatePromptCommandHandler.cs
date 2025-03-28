using FastEndpoints;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using PromptArchive.Features.Prompts.Get;

namespace PromptArchive.Features.Prompts.Create;

public class CreatePromptCommandHandler : ICommandHandler<CreatePromptCommand, Result<PromptResponse>>
{
    private readonly ILogger<CreatePromptCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public CreatePromptCommandHandler(ILogger<CreatePromptCommandHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<PromptResponse>> ExecuteAsync(CreatePromptCommand command, CancellationToken ct)
    {
        var prompt = new Prompt
        {
            Title = command.Title,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = command.User
        };

        if (command.Tags?.Count > 0)
        {
            foreach (var tagName in command.Tags.Distinct())
            {
                if (string.IsNullOrWhiteSpace(tagName))
                    continue;

                var normalizedName = tagName.ToLower();
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.NormalizedName == normalizedName,
                    cancellationToken: ct);

                if (tag is null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        NormalizedName = normalizedName
                    };

                    _dbContext.Tags.Add(tag);
                }

                prompt.Tags.Add(tag);
            }
        }

        _dbContext.Prompts.Add(prompt);
        await _dbContext.SaveChangesAsync(ct);

        var versionResult = await new CreatePromptVersionCommand().ExecuteAsync();
        if (versionResult.IsFailed)
            return Result.Fail(versionResult.Errors);

        return await new GetPromptByIdQuery(prompt.Id).ExecuteAsync();
    }
}
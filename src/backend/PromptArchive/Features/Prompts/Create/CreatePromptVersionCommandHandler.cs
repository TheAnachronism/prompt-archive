using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.Create;

public class CreatePromptVersionCommandHandler : ICommandHandler<CreatePromptVersionCommand, Result>
{
    private readonly ILogger<CreatePromptVersionCommandHandler> _logger;
    private readonly ApplicationDbContext _dbContext;
    
    public CreatePromptVersionCommandHandler(ILogger<CreatePromptVersionCommandHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public Task<Result> ExecuteAsync(CreatePromptVersionCommand command, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
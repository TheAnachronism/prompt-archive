using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.AddPromptComment;

public record AddPromptCommentCommand(Guid PromptId, ApplicationUser User, string Content) : ICommand<Result<PromptComment>>;

public record AddPromptCommentCommandHandler : ICommandHandler<AddPromptCommentCommand, Result<PromptComment>>
{
    private readonly ApplicationDbContext _dbContext;

    public AddPromptCommentCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PromptComment>> ExecuteAsync(AddPromptCommentCommand command, CancellationToken ct)
    {
        var prompt = await _dbContext.Prompts.FindAsync([command.PromptId], ct);
        if (prompt is null)
            return Result.Fail("Prompt not found");

        var comment = new PromptComment
        {
            Prompt = prompt,
            Content = command.Content,
            CreatedAt = DateTime.UtcNow,
            User = command.User
        };

        await _dbContext.PromptComments.AddAsync(comment, ct);
        await _dbContext.SaveChangesAsync(ct);

        return comment;
    }
}
using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.UpdatePromptComment;

public record UpdatePromptCommentCommand(Guid Id, string Content, string UserId) :
    ICommand<Result<PromptComment>>;

public class UpdatePromptCommentCommandHandler : ICommandHandler<UpdatePromptCommentCommand, Result<PromptComment>>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdatePromptCommentCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PromptComment>> ExecuteAsync(UpdatePromptCommentCommand command, CancellationToken ct)
    {
        var comment = await _dbContext.PromptComments.FindAsync([command.Id], ct);
        if (comment is null)
            return Result.Fail("Comment not found");

        if (comment.UserId != command.UserId)
            return Result.Fail("You can't update this comment");

        comment.Content = command.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        return comment;
    }
}
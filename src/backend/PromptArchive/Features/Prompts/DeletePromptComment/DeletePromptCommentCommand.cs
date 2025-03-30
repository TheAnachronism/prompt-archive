using FastEndpoints;
using FluentResults;
using PromptArchive.Database;

namespace PromptArchive.Features.Prompts.DeletePromptComment;

public record DeletePromptCommentCommand(Guid Id, string UserId, bool IsAdmin) : ICommand<Result>;

public class DeletePromptCommentCommandHandler : ICommandHandler<DeletePromptCommentCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public DeletePromptCommentCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(DeletePromptCommentCommand command, CancellationToken ct)
    {
        var comment = await _dbContext.PromptComments.FindAsync([command.Id], ct);
        if (comment is null)
            return Result.Fail("Comment not found");

        if (comment.UserId != command.UserId && !command.IsAdmin)
            return Result.Fail("You cannot delete this comment");

        _dbContext.PromptComments.Remove(comment);
        await _dbContext.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
using FastEndpoints;
using FluentResults;

namespace PromptArchive.Features.Prompts.Get;

public class GetPromptsCommand : ICommand<Result<PromptListResponse>>
{
    public GetPromptsCommand(int page, int pageSize, string? searchTerm, string? tag, string? userId)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        Tag = tag;
        UserId = userId;
    }

    public int Page { get; }
    public int PageSize { get;  }
    public string? SearchTerm { get;  }
    public string? Tag { get;  }
    public string? UserId { get;  }
}
using FastEndpoints;
using FluentValidation;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts.GetPrompt;

public class GetPromptByIdEndpoint : Endpoint<PromptIdRequest, PromptResponse>
{
    private readonly IStorageService _storageService;

    public GetPromptByIdEndpoint(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("prompts/{Id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PromptIdRequest req, CancellationToken ct)
    {
        var result = await new GetPromptByIdCommand(req.Id).ExecuteAsync(ct);
        if (result.IsFailed)
        {
            foreach (var error in result.Errors) AddError(error.Message);
            ThrowIfAnyErrors();
        }

        await SendOkAsync(result.Value.ToResponse(_storageService), ct);
    }
}
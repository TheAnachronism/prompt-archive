using FastEndpoints;
using FluentResults;

namespace PromptArchive.Extensions;

public static class ResultEndpointExtensions
{
    public static void ThrowIfAnyErrors<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint,
        IResultBase result) where TRequest : notnull
    {
        foreach (var error in result.Errors) endpoint.AddError(error.Message);
        endpoint.ThrowIfAnyErrors();
    }
}
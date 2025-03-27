using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.UserManagement.GetRoles;

public class GetRolesEndpoint : EndpointWithoutRequest<List<string>>
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<GetRolesEndpoint> _logger;

    public GetRolesEndpoint(RoleManager<IdentityRole> roleManager, ILogger<GetRolesEndpoint> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("roles");
        Policies("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await _roleManager.Roles.Select(x => x.Name)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToListAsync(cancellationToken: ct);

        await SendOkAsync(roles.Cast<string>().ToList(), cancellation: ct);
    }
}
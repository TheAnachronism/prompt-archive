using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Features.UserManagement.ListUsers;

public class ListUsersEndpoint : Endpoint<ListUsersRequest, UserListResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ListUsersEndpoint> _logger;

    public ListUsersEndpoint(UserManager<ApplicationUser> userManager, ILogger<ListUsersEndpoint> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("manage/users");
        Policies("Admin");
    }

    public override async Task HandleAsync(ListUsersRequest req, CancellationToken ct)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(req.SearchTerm))
        {
            var searchTerm = req.SearchTerm.ToLower();
            query = query.Where(u =>
                u.UserName!.ToLower().Contains(searchTerm) || u.Email!.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken: ct);

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync(ct);

        var userDtOs = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtOs.Add(new UserDto(user.Id, user.UserName!, user.Email!, roles.ToList(), user.EmailConfirmed,
                user.CreatedAt, user.LastLoginAt));
        }

        await SendAsync(new UserListResponse(userDtOs, totalCount, req.PageSize, req.Page), cancellation: ct);
    }
}
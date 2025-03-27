namespace PromptArchive.Features.UserManagement.ListUsers;

public record UserListResponse(List<UserDto> Users, int TotalCount, int PageSize, int CurentPage);
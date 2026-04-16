namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminManagedUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Roles { get; init; } = [];
}

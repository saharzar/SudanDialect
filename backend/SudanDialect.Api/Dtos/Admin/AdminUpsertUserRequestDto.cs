namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminUpsertUserRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
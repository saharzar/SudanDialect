namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminWordSuggestionPageDto
{
    public IReadOnlyList<AdminWordSuggestionItemDto> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}

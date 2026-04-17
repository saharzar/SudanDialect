using SudanDialect.Api.Dtos.Admin;

namespace SudanDialect.Api.Interfaces.Repositories;

public interface IAdminWordSuggestionRepository
{
    Task<(IReadOnlyList<AdminWordSuggestionItemDto> Items, int TotalCount)> GetPagedAsync(
        string? query,
        bool? resolved,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default);
}

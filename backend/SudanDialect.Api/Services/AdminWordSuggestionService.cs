using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Interfaces.Repositories;
using SudanDialect.Api.Interfaces.Services;

namespace SudanDialect.Api.Services;

public sealed class AdminWordSuggestionService : IAdminWordSuggestionService
{
    private const int MaxPageSize = 200;

    private readonly IAdminWordSuggestionRepository _adminWordSuggestionRepository;

    public AdminWordSuggestionService(IAdminWordSuggestionRepository adminWordSuggestionRepository)
    {
        _adminWordSuggestionRepository = adminWordSuggestionRepository;
    }

    public async Task<AdminWordSuggestionPageDto> GetPageAsync(
        AdminWordSuggestionQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, MaxPageSize);
        var sortDescending = NormalizeSortDirection(query.SortDirection) == "desc";

        var (items, totalCount) = await _adminWordSuggestionRepository.GetPagedAsync(
            query.Query,
            query.Resolved,
            sortDescending,
            page,
            pageSize,
            cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var boundedPage = totalPages == 0 ? 1 : Math.Min(page, totalPages);

        if (totalPages > 0 && page > totalPages)
        {
            (items, _) = await _adminWordSuggestionRepository.GetPagedAsync(
                query.Query,
                query.Resolved,
                sortDescending,
                boundedPage,
                pageSize,
                cancellationToken);
        }

        return new AdminWordSuggestionPageDto
        {
            Items = items,
            Page = boundedPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Suggestion id must be a positive integer.");
        }

        return await _adminWordSuggestionRepository.SetResolvedAsync(id, resolved, cancellationToken);
    }

    private static string NormalizeSortDirection(string? sortDirection)
    {
        if (string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
        {
            return "asc";
        }

        return "desc";
    }
}

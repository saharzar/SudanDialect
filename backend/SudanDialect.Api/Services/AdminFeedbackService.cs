using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Interfaces.Repositories;
using SudanDialect.Api.Interfaces.Services;

namespace SudanDialect.Api.Services;

public sealed class AdminFeedbackService : IAdminFeedbackService
{
    private const int MaxPageSize = 200;

    private readonly IAdminFeedbackRepository _adminFeedbackRepository;

    public AdminFeedbackService(IAdminFeedbackRepository adminFeedbackRepository)
    {
        _adminFeedbackRepository = adminFeedbackRepository;
    }

    public async Task<AdminFeedbackPageDto> GetPageAsync(AdminFeedbackQueryDto query, CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, MaxPageSize);

        if (query.WordId.HasValue && query.WordId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(query.WordId), "Word id filter must be a positive integer.");
        }

        var sortDescending = NormalizeSortDirection(query.SortDirection) == "desc";

        var (items, totalCount) = await _adminFeedbackRepository.GetPagedAsync(
            query.Resolved,
            query.WordId,
            sortDescending,
            page,
            pageSize,
            cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var boundedPage = totalPages == 0 ? 1 : Math.Min(page, totalPages);

        if (totalPages > 0 && page > totalPages)
        {
            (items, _) = await _adminFeedbackRepository.GetPagedAsync(
                query.Resolved,
                query.WordId,
                sortDescending,
                boundedPage,
                pageSize,
                cancellationToken);
        }

        return new AdminFeedbackPageDto
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
            throw new ArgumentOutOfRangeException(nameof(id), "Feedback id must be a positive integer.");
        }

        return await _adminFeedbackRepository.SetResolvedAsync(id, resolved, cancellationToken);
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

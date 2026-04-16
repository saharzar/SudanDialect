using SudanDialect.Api.Dtos.Admin;

namespace SudanDialect.Api.Interfaces.Repositories;

public interface IAdminFeedbackRepository
{
    Task<(IReadOnlyList<AdminFeedbackItemDto> Items, int TotalCount)> GetPagedAsync(
        bool? resolved,
        int? wordId,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default);
}

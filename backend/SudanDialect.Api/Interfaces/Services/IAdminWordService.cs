using SudanDialect.Api.Dtos;
using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Models;

namespace SudanDialect.Api.Interfaces.Services;

public interface IAdminWordService
{
    Task<AdminDashboardMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default);

    Task<WordPageDto> GetPageAsync(AdminWordTableQueryDto query, CancellationToken cancellationToken = default);

    Task<Word?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Word> CreateAsync(AdminCreateWordRequestDto request, CancellationToken cancellationToken = default);

    Task<Word?> UpdateAsync(
        int id,
        AdminUpdateWordRequestDto request,
        string adminUserId,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        int id,
        string adminUserId,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<AdminWordEditAuditPageDto> GetAuditPageAsync(
        AdminWordEditAuditQueryDto query,
        CancellationToken cancellationToken = default);

    Task<AdminWordEditAuditPageDto> GetAuditPageByWordIdAsync(
        int wordId,
        AdminWordEditAuditQueryDto query,
        CancellationToken cancellationToken = default);
}

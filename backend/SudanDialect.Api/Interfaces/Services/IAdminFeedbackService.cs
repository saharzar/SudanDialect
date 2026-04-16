using SudanDialect.Api.Dtos.Admin;

namespace SudanDialect.Api.Interfaces.Services;

public interface IAdminFeedbackService
{
    Task<AdminFeedbackPageDto> GetPageAsync(AdminFeedbackQueryDto query, CancellationToken cancellationToken = default);

    Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default);
}

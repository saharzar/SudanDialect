using SudanDialect.Api.Dtos.Admin;

namespace SudanDialect.Api.Interfaces.Services;

public interface IAdminWordSuggestionService
{
    Task<AdminWordSuggestionPageDto> GetPageAsync(AdminWordSuggestionQueryDto query, CancellationToken cancellationToken = default);

    Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default);
}

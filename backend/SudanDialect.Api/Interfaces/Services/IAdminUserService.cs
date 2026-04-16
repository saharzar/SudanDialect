using SudanDialect.Api.Dtos.Admin;

namespace SudanDialect.Api.Interfaces.Services;

public interface IAdminUserService
{
    Task<IReadOnlyCollection<AdminManagedUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AdminManagedUserDto> CreateAsync(AdminUpsertUserRequestDto request, CancellationToken cancellationToken = default);

    Task<AdminManagedUserDto?> UpdateAsync(string id, AdminUpsertUserRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

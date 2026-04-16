using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Interfaces.Services;
using SudanDialect.Api.Utilities;

namespace SudanDialect.Api.Services;

public sealed class AdminUserService : IAdminUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminUserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyCollection<AdminManagedUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var result = new List<AdminManagedUserDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await EnsureRolesAsync(user);
            result.Add(new AdminManagedUserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Roles = roles.ToArray()
            });
        }

        return result;
    }

    public async Task<AdminManagedUserDto> CreateAsync(AdminUpsertUserRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var username = NormalizeUsername(request.Username);
        var password = ValidatePassword(request.Password);

        var existingUser = await _userManager.FindByNameAsync(username);
        if (existingUser is not null)
        {
            throw new ArgumentException("Username already exists.", nameof(request.Username));
        }

        var user = new IdentityUser
        {
            UserName = username
        };

        var createResult = await _userManager.CreateAsync(user, password);
        EnsureIdentityResult(createResult, "Failed to create user.");

        var addRoleResult = await _userManager.AddToRoleAsync(user, AdminRoleNames.Moderator);
        EnsureIdentityResult(addRoleResult, "Failed assigning default role.");

        return new AdminManagedUserDto
        {
            Id = user.Id,
            Username = user.UserName ?? username,
            Roles = [AdminRoleNames.Moderator]
        };
    }

    public async Task<AdminManagedUserDto?> UpdateAsync(string id, AdminUpsertUserRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var username = NormalizeUsername(request.Username);
        var password = ValidatePassword(request.Password);

        var userWithSameName = await _userManager.FindByNameAsync(username);
        if (userWithSameName is not null && !string.Equals(userWithSameName.Id, user.Id, StringComparison.Ordinal))
        {
            throw new ArgumentException("Username already exists.", nameof(request.Username));
        }

        user.UserName = username;
        var updateResult = await _userManager.UpdateAsync(user);
        EnsureIdentityResult(updateResult, "Failed to update username.");

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
        EnsureIdentityResult(passwordResult, "Failed to update password.");

        var roles = await EnsureRolesAsync(user);

        return new AdminManagedUserDto
        {
            Id = user.Id,
            Username = user.UserName ?? username,
            Roles = roles.ToArray()
        };
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        var deleteResult = await _userManager.DeleteAsync(user);
        EnsureIdentityResult(deleteResult, "Failed to delete user.");
        return true;
    }

    private static string NormalizeUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        var normalized = username.Trim();
        if (normalized.Length < 3)
        {
            throw new ArgumentException("Username must be at least 3 characters.", nameof(username));
        }

        return normalized;
    }

    private static string ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }

        var normalized = password.Trim();
        if (normalized.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters.", nameof(password));
        }

        return normalized;
    }

    private async Task<IReadOnlyCollection<string>> EnsureRolesAsync(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count > 0)
        {
            return roles.ToArray();
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, AdminRoleNames.Moderator);
        EnsureIdentityResult(addRoleResult, "Failed assigning default role.");

        return [AdminRoleNames.Moderator];
    }

    private static void EnsureIdentityResult(IdentityResult result, string fallbackMessage)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
        throw new ArgumentException(string.IsNullOrWhiteSpace(errors) ? fallbackMessage : errors);
    }
}

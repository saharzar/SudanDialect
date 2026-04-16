using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SudanDialect.Api.Configuration;
using SudanDialect.Api.Utilities;

namespace SudanDialect.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminUsersAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("AdminUserSeeder");
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

        await EnsureRolesExistAsync(roleManager, logger);

        await EnsureExistingUsersHaveDefaultRoleAsync(userManager, logger, cancellationToken);

        if (options.Users.Count == 0)
        {
            logger.LogWarning("No admin seed users configured. Set AdminSeed:Users in configuration or user-secrets.");
            return;
        }

        foreach (var configuredUser in options.Users)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var username = configuredUser.Username.Trim();
            var password = configuredUser.Password;
            var configuredRole = ResolveRole(configuredUser.Role, logger);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Skipped admin seed entry because username or password is empty.");
                continue;
            }

            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser is not null)
            {
                await EnsureUserHasRoleAsync(userManager, existingUser, configuredRole, logger);
                logger.LogInformation("Admin user '{Username}' already exists.", username);
                continue;
            }

            var user = new IdentityUser
            {
                UserName = username
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await EnsureUserHasRoleAsync(userManager, user, configuredRole, logger);
                logger.LogInformation("Seeded admin user '{Username}'.", username);
                continue;
            }

            var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
            logger.LogError("Failed to seed admin user '{Username}'. Errors: {Errors}", username, errors);
        }
    }

    private static async Task EnsureRolesExistAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var roleName in AdminRoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var createResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (createResult.Succeeded)
            {
                logger.LogInformation("Seeded role '{Role}'.", roleName);
                continue;
            }

            var errors = string.Join("; ", createResult.Errors.Select(error => $"{error.Code}: {error.Description}"));
            logger.LogError("Failed to seed role '{Role}'. Errors: {Errors}", roleName, errors);
        }
    }

    private static async Task EnsureExistingUsersHaveDefaultRoleAsync(
        UserManager<IdentityUser> userManager,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var existingUsers = await userManager.Users.ToListAsync(cancellationToken);

        foreach (var user in existingUsers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                continue;
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, AdminRoleNames.Moderator);
            if (addRoleResult.Succeeded)
            {
                logger.LogInformation("Assigned default role '{Role}' to existing user '{Username}'.", AdminRoleNames.Moderator, user.UserName ?? user.Id);
                continue;
            }

            var errors = string.Join("; ", addRoleResult.Errors.Select(error => $"{error.Code}: {error.Description}"));
            logger.LogError(
                "Failed assigning default role '{Role}' to existing user '{Username}'. Errors: {Errors}",
                AdminRoleNames.Moderator,
                user.UserName ?? user.Id,
                errors);
        }
    }

    private static string ResolveRole(string? configuredRole, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(configuredRole))
        {
            return AdminRoleNames.Moderator;
        }

        var normalized = configuredRole.Trim().ToLowerInvariant();
        if (normalized is AdminRoleNames.Admin or AdminRoleNames.Moderator)
        {
            return normalized;
        }

        logger.LogWarning(
            "Configured role '{Role}' is not supported. Defaulting to '{DefaultRole}'.",
            configuredRole,
            AdminRoleNames.Moderator);

        return AdminRoleNames.Moderator;
    }

    private static async Task EnsureUserHasRoleAsync(
        UserManager<IdentityUser> userManager,
        IdentityUser user,
        string role,
        ILogger logger)
    {
        if (await userManager.IsInRoleAsync(user, role))
        {
            return;
        }

        var result = await userManager.AddToRoleAsync(user, role);
        if (result.Succeeded)
        {
            logger.LogInformation("Assigned role '{Role}' to user '{Username}'.", role, user.UserName ?? user.Id);
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
        logger.LogError("Failed assigning role '{Role}' to user '{Username}'. Errors: {Errors}", role, user.UserName ?? user.Id, errors);
    }
}

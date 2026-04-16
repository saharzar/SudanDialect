using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SudanDialect.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminModeratorRolesBootstrap : Migration
    {
        private const string AdminRoleId = "2f17c6d4-5f0e-4d59-a2e7-24f53cc4cfd1";
        private const string ModeratorRoleId = "4b7cbccf-90db-4f5f-8f95-32c96ecf63e4";
        private const string AdminRoleStamp = "0f06cd7c-7dc0-46db-a532-8ef5bf8fe903";
        private const string ModeratorRoleStamp = "59f557f1-f0c3-4f52-bf5a-e95f867871f0";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"""
                INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
                SELECT '{AdminRoleId}', 'admin', 'ADMIN', '{AdminRoleStamp}'
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "AspNetRoles"
                    WHERE "NormalizedName" = 'ADMIN'
                );
                """);

            migrationBuilder.Sql(
                $"""
                INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
                SELECT '{ModeratorRoleId}', 'moderator', 'MODERATOR', '{ModeratorRoleStamp}'
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "AspNetRoles"
                    WHERE "NormalizedName" = 'MODERATOR'
                );
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
                    SELECT users."Id", moderatorRole."Id"
                FROM "AspNetUsers" users
                    JOIN "AspNetRoles" moderatorRole ON moderatorRole."NormalizedName" = 'MODERATOR'
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "AspNetUserRoles" userRoles
                    WHERE userRoles."UserId" = users."Id"
                        AND userRoles."RoleId" = moderatorRole."Id"
                );
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "AspNetUserRoles"
                WHERE "RoleId" IN (
                    SELECT "Id"
                    FROM "AspNetRoles"
                    WHERE "NormalizedName" IN ('ADMIN', 'MODERATOR')
                );
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "AspNetRoles"
                WHERE "NormalizedName" IN ('ADMIN', 'MODERATOR');
                """);

        }
    }
}

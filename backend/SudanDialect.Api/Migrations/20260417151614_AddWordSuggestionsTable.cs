using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SudanDialect.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWordSuggestionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "word_suggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    headword = table.Column<string>(type: "text", nullable: false),
                    definition = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    resolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_word_suggestions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_word_suggestions_resolved_timestamp",
                table: "word_suggestions",
                columns: new[] { "resolved", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_word_suggestions_timestamp",
                table: "word_suggestions",
                column: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "word_suggestions");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SudanDialect.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WordId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<string>(type: "text", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActionType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    OldHeadword = table.Column<string>(type: "text", nullable: false),
                    NewHeadword = table.Column<string>(type: "text", nullable: false),
                    OldDefinition = table.Column<string>(type: "text", nullable: false),
                    NewDefinition = table.Column<string>(type: "text", nullable: false),
                    OldIsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NewIsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OldNormalizedHeadword = table.Column<string>(type: "text", nullable: false),
                    NewNormalizedHeadword = table.Column<string>(type: "text", nullable: false),
                    OldNormalizedDefinition = table.Column<string>(type: "text", nullable: false),
                    NewNormalizedDefinition = table.Column<string>(type: "text", nullable: false),
                    ClientIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_words_WordId",
                        column: x => x.WordId,
                        principalTable: "words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_AdminUserId_EditedAt",
                table: "Audits",
                columns: new[] { "AdminUserId", "EditedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_EditedAt",
                table: "Audits",
                column: "EditedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_WordId_EditedAt",
                table: "Audits",
                columns: new[] { "WordId", "EditedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");
        }
    }
}

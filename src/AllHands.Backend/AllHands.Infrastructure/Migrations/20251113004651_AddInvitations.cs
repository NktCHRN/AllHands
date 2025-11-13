using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssuesAt",
                table: "Sessions",
                newName: "IssuedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_IssuesAt",
                table: "Sessions",
                newName: "IX_Sessions_IssuedAt");

            migrationBuilder.CreateTable(
                name: "Invitation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IssuerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_ExpiresAt_TokenHash",
                table: "Invitation",
                columns: new[] { "ExpiresAt", "TokenHash" });

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_IssuerId",
                table: "Invitation",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_UserId",
                table: "Invitation",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitation");

            migrationBuilder.RenameColumn(
                name: "IssuedAt",
                table: "Sessions",
                newName: "IssuesAt");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_IssuedAt",
                table: "Sessions",
                newName: "IX_Sessions_IssuesAt");
        }
    }
}

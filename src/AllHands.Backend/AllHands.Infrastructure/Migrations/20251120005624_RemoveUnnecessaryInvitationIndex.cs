using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryInvitationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invitations_ExpiresAt_TokenHash",
                table: "Invitations");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "Invitations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "Invitations");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ExpiresAt_TokenHash",
                table: "Invitations",
                columns: new[] { "ExpiresAt", "TokenHash" });
        }
    }
}

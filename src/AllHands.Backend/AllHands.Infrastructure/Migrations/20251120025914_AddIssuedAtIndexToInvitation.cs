using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIssuedAtIndexToInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invitations_IssuedAt",
                table: "Invitations",
                column: "IssuedAt",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invitations_IssuedAt",
                table: "Invitations");
        }
    }
}

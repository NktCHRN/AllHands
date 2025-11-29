using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesIsDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AspNetRoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CompanyId",
                table: "AspNetRoles",
                column: "CompanyId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL AND \"IsDefault\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CompanyId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AspNetRoles");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIsRevoked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "Sessions");
        }
    }
}

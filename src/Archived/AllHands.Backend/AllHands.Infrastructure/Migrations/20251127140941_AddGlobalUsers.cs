using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GlobalUserId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GlobalUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DefaultCompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GlobalUserId",
                table: "AspNetUsers",
                column: "GlobalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GlobalUsers_GlobalUserId",
                table: "AspNetUsers",
                column: "GlobalUserId",
                principalTable: "GlobalUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GlobalUsers_GlobalUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "GlobalUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GlobalUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GlobalUserId",
                table: "AspNetUsers");
        }
    }
}

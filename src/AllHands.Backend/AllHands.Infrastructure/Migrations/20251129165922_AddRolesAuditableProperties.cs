using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAuditableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "AspNetRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "AspNetRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "AspNetRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CreatedByUserId",
                table: "AspNetRoles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_DeletedByUserId",
                table: "AspNetRoles",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_UpdatedByUserId",
                table: "AspNetRoles",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_CreatedByUserId",
                table: "AspNetRoles",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_DeletedByUserId",
                table: "AspNetRoles",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UpdatedByUserId",
                table: "AspNetRoles",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_CreatedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_DeletedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_AspNetUsers_UpdatedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CreatedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_DeletedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_UpdatedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AspNetRoles");
        }
    }
}

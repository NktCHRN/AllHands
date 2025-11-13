using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllHands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_AspNetUsers_IssuerId",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_AspNetUsers_UserId",
                table: "Invitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation");

            migrationBuilder.RenameTable(
                name: "Invitation",
                newName: "Invitations");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_UserId",
                table: "Invitations",
                newName: "IX_Invitations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_IssuerId",
                table: "Invitations",
                newName: "IX_Invitations_IssuerId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_ExpiresAt_TokenHash",
                table: "Invitations",
                newName: "IX_Invitations_ExpiresAt_TokenHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_AspNetUsers_IssuerId",
                table: "Invitations",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_AspNetUsers_UserId",
                table: "Invitations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_AspNetUsers_IssuerId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_AspNetUsers_UserId",
                table: "Invitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations");

            migrationBuilder.RenameTable(
                name: "Invitations",
                newName: "Invitation");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_UserId",
                table: "Invitation",
                newName: "IX_Invitation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_IssuerId",
                table: "Invitation",
                newName: "IX_Invitation_IssuerId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_ExpiresAt_TokenHash",
                table: "Invitation",
                newName: "IX_Invitation_ExpiresAt_TokenHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_AspNetUsers_IssuerId",
                table: "Invitation",
                column: "IssuerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_AspNetUsers_UserId",
                table: "Invitation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

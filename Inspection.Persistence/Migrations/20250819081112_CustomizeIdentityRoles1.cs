using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CustomizeIdentityRoles1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_SystematicUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_SystematicUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "SystematicUserId",
                table: "AspNetUserRoles");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "IdentityUserRole<string>",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoleId1",
                table: "IdentityUserRole<string>",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "IdentityUserRole<string>",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole<string>_RoleId1",
                table: "IdentityUserRole<string>",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole<string>_UserId1",
                table: "IdentityUserRole<string>",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_AspNetRoles_RoleId1",
                table: "IdentityUserRole<string>",
                column: "RoleId1",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_AspNetUsers_UserId1",
                table: "IdentityUserRole<string>",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserRole<string>_AspNetRoles_RoleId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityUserRole<string>_AspNetUsers_UserId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropIndex(
                name: "IX_IdentityUserRole<string>_RoleId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropIndex(
                name: "IX_IdentityUserRole<string>_UserId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "IdentityUserRole<string>");

            migrationBuilder.AddColumn<string>(
                name: "SystematicUserId",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_SystematicUserId",
                table: "AspNetUserRoles",
                column: "SystematicUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_SystematicUserId",
                table: "AspNetUserRoles",
                column: "SystematicUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ManageRequests_14031220 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformerContactPerson",
                table: "Circulations");

            migrationBuilder.AddColumn<Guid>(
                name: "PerformerContactPersonContactId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_PerformerContactPersonContactId",
                table: "Circulations",
                column: "PerformerContactPersonContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations",
                column: "PerformerContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_PerformerContactPersonContactId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "PerformerContactPersonContactId",
                table: "Circulations");

            migrationBuilder.AddColumn<string>(
                name: "PerformerContactPerson",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

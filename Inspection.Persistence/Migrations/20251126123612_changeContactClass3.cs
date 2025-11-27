using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeContactClass3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorContactPersonContactId",
                table: "Requests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests",
                column: "CreatorContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorContactPersonContactId",
                table: "Requests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests",
                column: "CreatorContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

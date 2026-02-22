using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatorContactId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionReports_Contacts_CreatorContactId",
            //    table: "InspectionReports");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorContactId",
                table: "InspectionReports",
                type: "uniqueidentifier",
                nullable: true);
               // oldClrType: typeof(Guid),
                //oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_Contacts_CreatorContactId",
                table: "InspectionReports",
                column: "CreatorContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_Contacts_CreatorContactId",
                table: "InspectionReports");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorContactId",
                table: "InspectionReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_Contacts_CreatorContactId",
                table: "InspectionReports",
                column: "CreatorContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

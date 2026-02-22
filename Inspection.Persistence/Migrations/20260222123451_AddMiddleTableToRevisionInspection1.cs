using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMiddleTableToRevisionInspection1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Revisions_RevisionId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_RevisionId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "RevisionId",
                table: "InspectionNotifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RevisionId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_RevisionId",
                table: "InspectionNotifications",
                column: "RevisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Revisions_RevisionId",
                table: "InspectionNotifications",
                column: "RevisionId",
                principalTable: "Revisions",
                principalColumn: "RevisionId");
        }
    }
}

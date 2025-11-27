using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeContactClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_InspectionNotificationId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "InspectionNotificationId",
                table: "Attachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InspectionNotificationId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_InspectionNotificationId",
                table: "Attachments",
                column: "InspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInspection_CirculationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Circulations_CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorContactPersonContactId",
                table: "Requests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionNotificationId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreatorContactPersonContactId",
                table: "Requests",
                column: "CreatorContactPersonContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_InspectionNotificationId",
                table: "Circulations",
                column: "InspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
                table: "Circulations",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests",
                column: "CreatorContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Contacts_CreatorContactPersonContactId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_CreatorContactPersonContactId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "CreatorContactPersonContactId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.AddColumn<Guid>(
                name: "CirculationId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_CirculationId",
                table: "InspectionNotifications",
                column: "CirculationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Circulations_CirculationId",
                table: "InspectionNotifications",
                column: "CirculationId",
                principalTable: "Circulations",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class img_ChangeRequest_CirculationTracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
            //    table: "Circulations");

            //migrationBuilder.DropIndex(
            //    name: "IX_Circulations_InspectionNotificationId",
            //    table: "Circulations");

            //migrationBuilder.DropColumn(
            //    name: "InspectionNotificationId",
            //    table: "Circulations");

            migrationBuilder.AddColumn<Guid>(
                name: "CirculationId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_CirculationId",
                table: "InspectionNotifications",
                column: "CirculationId");

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_RequestId",
                table: "Circulations",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_Requests_RequestId",
                table: "Circulations",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Circulations_CirculationId",
                table: "InspectionNotifications",
                column: "CirculationId",
                principalTable: "Circulations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Requests_RequestId",
                table: "Circulations");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Circulations_CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_RequestId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "CirculationId",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionNotificationId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_InspectionNotificationId",
                table: "Circulations",
                column: "InspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
                table: "Circulations",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId");
        }
    }
}

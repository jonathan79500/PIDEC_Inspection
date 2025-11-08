using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRequestRelations1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId1",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropIndex(
            //    name: "IX_InspectionNotifications_RequestId1",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropColumn(
            //    name: "RequestId1",
            //    table: "InspectionNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Requests_RequestId",
                table: "Circulations");


            migrationBuilder.DropIndex(
                name: "IX_Circulations_RequestId",
                table: "Circulations");
           

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Circulations");


            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Requests_RequestId",
                table: "InspectionNotifications",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId",
            //    table: "InspectionNotifications",
            //    column: "RequestId",
            //    principalTable: "Requests",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId",
            //    table: "InspectionNotifications");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "RequestId1",
            //    table: "InspectionNotifications",
            //    type: "uniqueidentifier",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_InspectionNotifications_RequestId1",
            //    table: "InspectionNotifications",
            //    column: "RequestId1");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId",
            //    table: "InspectionNotifications",
            //    column: "RequestId",
            //    principalTable: "Requests",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InspectionNotifications_Requests_RequestId1",
            //    table: "InspectionNotifications",
            //    column: "RequestId1",
            //    principalTable: "Requests",
            //    principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RevisionTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_RequestId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<string>(
                name: "CurrentStatus",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Requests");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_RequestId",
                table: "InspectionNotifications",
                column: "RequestId");
           
        }
    }
}

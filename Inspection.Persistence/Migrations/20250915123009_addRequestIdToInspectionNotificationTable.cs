using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addRequestIdToInspectionNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "QCActionType",
                table: "Circulations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleCode",
                table: "AspNetRoles",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            //migrationBuilder.CreateIndex(
            //    name: "IX_InspectionNotifications_RequestId",
            //    table: "InspectionNotifications",
            //    column: "RequestId");

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
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Requests_RequestId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_RequestId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "QCActionType",
                table: "Circulations");

            migrationBuilder.AlterColumn<string>(
                name: "RoleCode",
                table: "AspNetRoles",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);
        }
    }
}

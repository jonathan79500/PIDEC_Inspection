using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeAttachmentDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionNotificationId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Base64Content",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Base64Content",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "Attachments");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionNotificationId",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                columns: new[] { "Id", "InspectionNotificationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_InspectionNotifications_InspectionNotificationId",
                table: "Attachments",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NotificationRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_ActionTypes_ActionTypeId",
                table: "Circulations");

            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_ActionTypeId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "Circulations");

            migrationBuilder.AlterColumn<int>(
                name: "SequenceNo",
                table: "Circulations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "PerformerPN",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PerformerContactType",
                table: "Circulations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "PerformerContactPersonContactId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PerformedActionDateGregorian",
                table: "Circulations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "PerformedActionDate",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "Circulations",
                type: "int",
                nullable: true);

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
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations",
                column: "PerformerContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
                table: "Circulations",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations");

            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_InspectionNotifications_InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "InspectionNotificationId",
                table: "Circulations");

            migrationBuilder.AlterColumn<int>(
                name: "SequenceNo",
                table: "Circulations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PerformerPN",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PerformerContactType",
                table: "Circulations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PerformerContactPersonContactId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PerformedActionDateGregorian",
                table: "Circulations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PerformedActionDate",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActionTypeId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_ActionTypes_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "ActionTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_Contacts_PerformerContactPersonContactId",
                table: "Circulations",
                column: "PerformerContactPersonContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

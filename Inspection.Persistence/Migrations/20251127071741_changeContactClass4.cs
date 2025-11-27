using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeContactClass4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationContacts_Contacts_ContactId",
                table: "InspectionNotificationContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationContacts_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionNotificationId",
                table: "InspectionNotificationContacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "InspectionNotificationContacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationContacts_Contacts_ContactId",
                table: "InspectionNotificationContacts",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationContacts_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationContacts",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationContacts_Contacts_ContactId",
                table: "InspectionNotificationContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationContacts_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationContacts");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionNotificationId",
                table: "InspectionNotificationContacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ContactId",
                table: "InspectionNotificationContacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationContacts_Contacts_ContactId",
                table: "InspectionNotificationContacts",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationContacts_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationContacts",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

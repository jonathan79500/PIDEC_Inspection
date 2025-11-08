using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class circulationChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationSubjects_InspectionSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionSubjectId",
                table: "InspectionNotificationSubjects",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Contacts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Circulations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_AspNetUsers_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationSubjects_InspectionSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects",
                column: "InspectionSubjectId",
                principalTable: "InspectionSubjects",
                principalColumn: "InspectionSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_AspNetUsers_UserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationSubjects_InspectionSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Circulations");

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionSubjectId",
                table: "InspectionNotificationSubjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationSubjects_InspectionSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects",
                column: "InspectionSubjectId",
                principalTable: "InspectionSubjects",
                principalColumn: "InspectionSubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

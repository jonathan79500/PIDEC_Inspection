using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RevisionTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.DropColumn(
                name: "Version",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentRevisionId",
                table: "Requests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RevisionId",
                table: "InspectionReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "RevisionId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RevisionId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Revisions",
                columns: table => new
                {
                    RevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedByContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revisions", x => x.RevisionId);
                    table.ForeignKey(
                        name: "FK_Revisions_Contacts_SubmittedByContactId",
                        column: x => x.SubmittedByContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Revisions_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReports_RevisionId",
                table: "InspectionReports",
                column: "RevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_RevisionId",
                table: "InspectionNotifications",
                column: "RevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_RevisionId",
                table: "Circulations",
                column: "RevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Revisions_RequestId",
                table: "Revisions",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Revisions_SubmittedByContactId",
                table: "Revisions",
                column: "SubmittedByContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_Revisions_RevisionId",
                table: "Circulations",
                column: "RevisionId",
                principalTable: "Revisions",
                principalColumn: "RevisionId",
                onDelete: ReferentialAction.Cascade);

          

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Revisions_RevisionId",
                table: "InspectionNotifications",
                column: "RevisionId",
                principalTable: "Revisions",
                principalColumn: "RevisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_Revisions_RevisionId",
                table: "InspectionReports",
                column: "RevisionId",
                principalTable: "Revisions",
                principalColumn: "RevisionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_Revisions_RevisionId",
                table: "Circulations");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Requests_RequestId",
                table: "InspectionNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Revisions_RevisionId",
                table: "InspectionNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_Revisions_RevisionId",
                table: "InspectionReports");

            migrationBuilder.DropTable(
                name: "Revisions");

            migrationBuilder.DropIndex(
                name: "IX_InspectionReports_RevisionId",
                table: "InspectionReports");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_RevisionId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_RevisionId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "CurrentRevisionId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RevisionId",
                table: "InspectionReports");

            migrationBuilder.DropColumn(
                name: "RevisionId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "RevisionId",
                table: "Circulations");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "InspectionNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

           
        }
    }
}

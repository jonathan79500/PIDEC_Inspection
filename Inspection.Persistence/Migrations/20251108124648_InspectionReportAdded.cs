using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InspectionReportAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InspectionReportId",
                table: "Contacts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionReportNumber",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InfoPart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReportNumber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionReports",
                columns: table => new
                {
                    InspectionReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionReportNumberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    RelatedInspectionNotificationInspectionNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReports", x => x.InspectionReportId);
                    table.ForeignKey(
                        name: "FK_InspectionReports_InspectionNotifications_RelatedInspectionNotificationInspectionNotificationId",
                        column: x => x.RelatedInspectionNotificationInspectionNotificationId,
                        principalTable: "InspectionNotifications",
                        principalColumn: "InspectionNotificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionReports_InspectionReportNumber_InspectionReportNumberId",
                        column: x => x.InspectionReportNumberId,
                        principalTable: "InspectionReportNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionReportAttachments",
                columns: table => new
                {
                    InspectionReportAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionReportId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    edmsId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PDFSize = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReportAttachments", x => x.InspectionReportAttachmentId);
                    table.ForeignKey(
                        name: "FK_InspectionReportAttachments_InspectionReports_InspectionReportId1",
                        column: x => x.InspectionReportId1,
                        principalTable: "InspectionReports",
                        principalColumn: "InspectionReportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_InspectionReportId",
                table: "Contacts",
                column: "InspectionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReportAttachments_InspectionReportId1",
                table: "InspectionReportAttachments",
                column: "InspectionReportId1");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReports_InspectionReportNumberId",
                table: "InspectionReports",
                column: "InspectionReportNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReports_RelatedInspectionNotificationInspectionNotificationId",
                table: "InspectionReports",
                column: "RelatedInspectionNotificationInspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_InspectionReports_InspectionReportId",
                table: "Contacts",
                column: "InspectionReportId",
                principalTable: "InspectionReports",
                principalColumn: "InspectionReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_InspectionReports_InspectionReportId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "InspectionReportAttachments");

            migrationBuilder.DropTable(
                name: "InspectionReports");

            migrationBuilder.DropTable(
                name: "InspectionReportNumber");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_InspectionReportId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "InspectionReportId",
                table: "Contacts");
        }
    }
}

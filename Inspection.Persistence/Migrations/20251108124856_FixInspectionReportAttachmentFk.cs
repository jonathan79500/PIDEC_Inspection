using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixInspectionReportAttachmentFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReportAttachments_InspectionReports_InspectionReportId1",
                table: "InspectionReportAttachments");

            migrationBuilder.RenameColumn(
                name: "InspectionReportId1",
                table: "InspectionReportAttachments",
                newName: "InspectionReportId");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionReportAttachments_InspectionReportId1",
                table: "InspectionReportAttachments",
                newName: "IX_InspectionReportAttachments_InspectionReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReportAttachments_InspectionReports_InspectionReportId",
                table: "InspectionReportAttachments",
                column: "InspectionReportId",
                principalTable: "InspectionReports",
                principalColumn: "InspectionReportId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReportAttachments_InspectionReports_InspectionReportId",
                table: "InspectionReportAttachments");

            migrationBuilder.RenameColumn(
                name: "InspectionReportId",
                table: "InspectionReportAttachments",
                newName: "InspectionReportId1");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionReportAttachments_InspectionReportId",
                table: "InspectionReportAttachments",
                newName: "IX_InspectionReportAttachments_InspectionReportId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReportAttachments_InspectionReports_InspectionReportId1",
                table: "InspectionReportAttachments",
                column: "InspectionReportId1",
                principalTable: "InspectionReports",
                principalColumn: "InspectionReportId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

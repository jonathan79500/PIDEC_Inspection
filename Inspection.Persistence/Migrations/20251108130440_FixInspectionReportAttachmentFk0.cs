using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixInspectionReportAttachmentFk0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_InspectionNotifications_RelatedInspectionNotificationInspectionNotificationId",
                table: "InspectionReports");

            migrationBuilder.RenameColumn(
                name: "RelatedInspectionNotificationInspectionNotificationId",
                table: "InspectionReports",
                newName: "RelatedInspectionNotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionReports_RelatedInspectionNotificationInspectionNotificationId",
                table: "InspectionReports",
                newName: "IX_InspectionReports_RelatedInspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_InspectionNotifications_RelatedInspectionNotificationId",
                table: "InspectionReports",
                column: "RelatedInspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_InspectionNotifications_RelatedInspectionNotificationId",
                table: "InspectionReports");

            migrationBuilder.RenameColumn(
                name: "RelatedInspectionNotificationId",
                table: "InspectionReports",
                newName: "RelatedInspectionNotificationInspectionNotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionReports_RelatedInspectionNotificationId",
                table: "InspectionReports",
                newName: "IX_InspectionReports_RelatedInspectionNotificationInspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_InspectionNotifications_RelatedInspectionNotificationInspectionNotificationId",
                table: "InspectionReports",
                column: "RelatedInspectionNotificationInspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

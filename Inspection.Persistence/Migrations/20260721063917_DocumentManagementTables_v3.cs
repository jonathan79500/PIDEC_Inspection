using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentManagementTables_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationInspectors_InspectionNotifications_InspectionNotificationId1",
                table: "InspectionNotificationInspectors");

            migrationBuilder.RenameColumn(
                name: "InspectionNotificationId1",
                table: "InspectionNotificationInspectors",
                newName: "InspectionNotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionNotificationInspectors_InspectionNotificationId1",
                table: "InspectionNotificationInspectors",
                newName: "IX_InspectionNotificationInspectors_InspectionNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationInspectors_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationInspectors",
                column: "InspectionNotificationId",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationInspectors_InspectionNotifications_InspectionNotificationId",
                table: "InspectionNotificationInspectors");

            migrationBuilder.RenameColumn(
                name: "InspectionNotificationId",
                table: "InspectionNotificationInspectors",
                newName: "InspectionNotificationId1");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionNotificationInspectors_InspectionNotificationId",
                table: "InspectionNotificationInspectors",
                newName: "IX_InspectionNotificationInspectors_InspectionNotificationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationInspectors_InspectionNotifications_InspectionNotificationId1",
                table: "InspectionNotificationInspectors",
                column: "InspectionNotificationId1",
                principalTable: "InspectionNotifications",
                principalColumn: "InspectionNotificationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

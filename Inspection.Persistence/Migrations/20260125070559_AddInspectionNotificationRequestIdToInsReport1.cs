using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionNotificationRequestIdToInsReport1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
              name: "RequestedInspectionNotificationId",
              table: "InspectionReports",
              newName: "RequestIdOfRelatedInspectionNotification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
             name: "RequestedInspectionNotificationId",
             table: "InspectionReports",
             newName: "RequestIdOfRelatedInspectionNotification");
        }
    }
}

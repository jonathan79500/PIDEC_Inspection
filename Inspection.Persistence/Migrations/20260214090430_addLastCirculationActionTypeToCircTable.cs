using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addLastCirculationActionTypeToCircTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_InspectionReportNumber_InspectionReportNumberId",
                table: "InspectionReports");

       

            migrationBuilder.RenameColumn(
                name: "InspectionReportNumberId",
                table: "InspectionReports",
                newName: "InspectionReportNumberPartsId");
     

            migrationBuilder.AddColumn<string>(
                name: "InspectionReportNumber",
                table: "InspectionReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PreviousActionType",
                table: "Circulations",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_InspectionReportNumber_InspectionReportNumberPartsId",
                table: "InspectionReports",
                column: "InspectionReportNumberPartsId",
                principalTable: "InspectionReportNumber",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_InspectionReportNumber_InspectionReportNumberPartsId",
                table: "InspectionReports");

            migrationBuilder.DropColumn(
                name: "InspectionReportNumber",
                table: "InspectionReports");

            migrationBuilder.DropColumn(
                name: "PreviousActionType",
                table: "Circulations");

            migrationBuilder.RenameColumn(
                name: "RequestIdOfRelatedInspectionNotification",
                table: "InspectionReports",
                newName: "RequestedInspectionNotificationId");

            migrationBuilder.RenameColumn(
                name: "InspectionReportNumberPartsId",
                table: "InspectionReports",
                newName: "InspectionReportNumberId");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionReports_InspectionReportNumberPartsId",
                table: "InspectionReports",
                newName: "IX_InspectionReports_InspectionReportNumberId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionReports_InspectionReportNumber_InspectionReportNumberId",
                table: "InspectionReports",
                column: "InspectionReportNumberId",
                principalTable: "InspectionReportNumber",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

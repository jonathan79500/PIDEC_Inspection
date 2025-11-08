using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrateInspectionNotification02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "RequestInspectionDueDate",
               table: "InspectionNotifications",
               type: "int",               
               nullable: true
               );

            migrationBuilder.AddColumn<string>(
              name: "RequestInspectionTimesInPeriod",
              table: "InspectionNotifications",
              type: "int",
              nullable: true
              );

            migrationBuilder.AddColumn<string>(
              name: "RequestInspectionPeriodDays",
              table: "InspectionNotifications",
              type: "int",
              nullable: true
              );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "RequestInspectionDueDate",
               table: "InspectionNotifications",
               type: "int",
               nullable: true
               );

            migrationBuilder.AddColumn<string>(
              name: "RequestInspectionTimesInPeriod",
              table: "InspectionNotifications",
              type: "int",
              nullable: true
              );

            migrationBuilder.AddColumn<string>(
              name: "RequestInspectionPeriodDays",
              table: "InspectionNotifications",
              type: "int",
              nullable: true
              );
        }
    }
}

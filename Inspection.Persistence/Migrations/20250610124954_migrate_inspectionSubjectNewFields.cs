using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migrate_inspectionSubjectNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "InspectionTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleByDefault",
                table: "InspectionTypes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "InspectionSubjects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleByDefault",
                table: "InspectionSubjects",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "InspectionTypes");

            migrationBuilder.DropColumn(
                name: "VisibleByDefault",
                table: "InspectionTypes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "InspectionSubjects");

            migrationBuilder.DropColumn(
                name: "VisibleByDefault",
                table: "InspectionSubjects");
        }
    }
}

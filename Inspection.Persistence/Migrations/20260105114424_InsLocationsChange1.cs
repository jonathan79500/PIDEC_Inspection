using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InsLocationsChange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotifications_Addresses_LocationId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_LocationId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentTagNo",
                table: "InspectionNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderName",
                table: "InspectionNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionNotificationLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionNotificationLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionNotificationLocations_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionNotificationLocations_InspectionNotifications_InspectionNotificationId",
                        column: x => x.InspectionNotificationId,
                        principalTable: "InspectionNotifications",
                        principalColumn: "InspectionNotificationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotificationLocations_AddressId",
                table: "InspectionNotificationLocations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotificationLocations_InspectionNotificationId",
                table: "InspectionNotificationLocations",
                column: "InspectionNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionNotificationLocations");

            migrationBuilder.DropColumn(
                name: "EquipmentTagNo",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "OrderName",
                table: "InspectionNotifications");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_LocationId",
                table: "InspectionNotifications",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_Addresses_LocationId",
                table: "InspectionNotifications",
                column: "LocationId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}

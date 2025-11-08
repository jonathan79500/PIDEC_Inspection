using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ManageRequests_14031220_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "InspectionNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ActionTypeId",
                table: "Circulations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ActionType",
                columns: table => new
                {
                    ActionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionType", x => x.ActionTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Circulations_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_ActionType_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId",
                principalTable: "ActionType",
                principalColumn: "ActionTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_ActionType_ActionTypeId",
                table: "Circulations");

            migrationBuilder.DropTable(
                name: "ActionType");

            migrationBuilder.DropIndex(
                name: "IX_Circulations_ActionTypeId",
                table: "Circulations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "Circulations");
        }
    }
}

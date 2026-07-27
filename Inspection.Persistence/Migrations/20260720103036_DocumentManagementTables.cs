using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectorOrders");

            migrationBuilder.DropTable(
                name: "InspectorProjects");

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionCompanyContactId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InspectorId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Persons",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_InspectionCompanyContactId",
                table: "Persons",
                column: "InspectionCompanyContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Contacts_InspectionCompanyContactId",
                table: "Persons",
                column: "InspectionCompanyContactId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Contacts_InspectionCompanyContactId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_InspectionCompanyContactId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "InspectionCompanyContactId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "InspectorId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Persons");

            migrationBuilder.CreateTable(
                name: "InspectorOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectorOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectorOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectorOrders_Persons_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectorProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectorProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectorProjects_Persons_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectorProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectorOrders_InspectorId",
                table: "InspectorOrders",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectorOrders_OrderId",
                table: "InspectorOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectorProjects_InspectorId",
                table: "InspectorProjects",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectorProjects_ProjectId",
                table: "InspectorProjects",
                column: "ProjectId");
        }
    }
}

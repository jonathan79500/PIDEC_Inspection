using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentManagementTables_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_CirculationInspectors_Persons_InspectorId",
            //    table: "CirculationInspectors");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotificationInspectors_Persons_InspectorId",
            //    table: "InspectionNotificationInspectors");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Persons_Contacts_InspectionCompanyContactId",
            //    table: "Persons");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Persons_InspectionReports_InspectionReportId",
            //    table: "Persons");

            //migrationBuilder.DropIndex(
            //    name: "IX_Persons_InspectionCompanyContactId",
            //    table: "Persons");

            //migrationBuilder.DropIndex(
            //    name: "IX_Persons_InspectionReportId",
            //    table: "Persons");

            //migrationBuilder.DropColumn(
            //    name: "Discriminator",
            //    table: "Persons");

            //migrationBuilder.DropColumn(
            //    name: "InspectionCompanyContactId",
            //    table: "Persons");

            //migrationBuilder.DropColumn(
            //    name: "InspectionReportId",
            //    table: "Persons");

            //migrationBuilder.DropColumn(
            //    name: "InspectorId",
            //    table: "Persons");

            //migrationBuilder.DropColumn(
            //    name: "IsActive",
            //    table: "Persons");
            migrationBuilder.DropTable(
               name: "Inspectors");

           migrationBuilder.CreateTable(
                name: "Inspectors",
                columns: table => new
                {
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionCompanyContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspectors", x => x.InspectorId);
                    table.ForeignKey(
                        name: "FK_Inspectors_Contacts_InspectionCompanyContactId",
                        column: x => x.InspectionCompanyContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspectors_InspectionReports_InspectionReportId",
                        column: x => x.InspectionReportId,
                        principalTable: "InspectionReports",
                        principalColumn: "InspectionReportId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspectors_InspectionCompanyContactId",
                table: "Inspectors",
                column: "InspectionCompanyContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspectors_InspectionReportId",
                table: "Inspectors",
                column: "InspectionReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationInspectors_Inspectors_InspectorId",
                table: "CirculationInspectors",
                column: "InspectorId",
                principalTable: "Inspectors",
                principalColumn: "InspectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotificationInspectors_Inspectors_InspectorId",
                table: "InspectionNotificationInspectors",
                column: "InspectorId",
                principalTable: "Inspectors",
                principalColumn: "InspectorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CirculationInspectors_Inspectors_InspectorId",
                table: "CirculationInspectors");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionNotificationInspectors_Inspectors_InspectorId",
                table: "InspectionNotificationInspectors");

            migrationBuilder.DropTable(
                name: "Inspectors");

            //migrationBuilder.AddColumn<string>(
            //    name: "Discriminator",
            //    table: "Persons",
            //    type: "nvarchar(13)",
            //    maxLength: 13,
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionCompanyContactId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionReportId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "InspectorId",
            //    table: "Persons",
            //    type: "uniqueidentifier",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsActive",
            //    table: "Persons",
            //    type: "bit",
            //    nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_InspectionCompanyContactId",
                table: "Persons",
                column: "InspectionCompanyContactId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Persons_InspectionReportId",
            //    table: "Persons",
            //    column: "InspectionReportId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CirculationInspectors_Persons_InspectorId",
            //    table: "CirculationInspectors",
            //    column: "InspectorId",
            //    principalTable: "Persons",
            //    principalColumn: "Id");

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_InspectionNotificationInspectors_Persons_InspectorId",
        //        table: "InspectionNotificationInspectors",
        //        column: "InspectorId",
        //        principalTable: "Persons",
        //        principalColumn: "Id",
        //        onDelete: ReferentialAction.Cascade);

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_Persons_Contacts_InspectionCompanyContactId",
        //        table: "Persons",
        //        column: "InspectionCompanyContactId",
        //        principalTable: "Contacts",
        //        principalColumn: "ContactId",
        //        onDelete: ReferentialAction.Cascade);

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_Persons_InspectionReports_InspectionReportId",
        //        table: "Persons",
        //        column: "InspectionReportId",
        //        principalTable: "InspectionReports",
        //        principalColumn: "InspectionReportId");
        }
    }
}

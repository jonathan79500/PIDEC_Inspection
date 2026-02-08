using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReportCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectorAttachments");


            migrationBuilder.AddColumn<string>(
                name: "InspectorType",
                table: "InspectionReportNumber",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReportNumber",
                table: "InspectionReportNumber",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");



            migrationBuilder.CreateTable(
              name: "Persons",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  Name = table.Column<string>(type: "nvarchar(150)", nullable: true),
                  FirstName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                  LastName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                  Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                  Position = table.Column<string>(type: "nvarchar(250)", nullable: true),
                  Phone = table.Column<string>(type: "nvarchar(150)", nullable: true),
                  Email = table.Column<string>(type: "nvarchar(250)", nullable: true),
              }
            );


            migrationBuilder.AddPrimaryKey(
                name: "PK_Persons",
                table: "Persons",
                column: "Id");



            migrationBuilder.CreateTable(
                name: "InspectionReportPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReportPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionReportPersons_InspectionReports_InspectionReportId",
                        column: x => x.InspectionReportId,
                        principalTable: "InspectionReports",
                        principalColumn: "InspectionReportId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionReportPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "PersonAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonAttachments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.CreateIndex(
                name: "IX_InspectionReportPersons_InspectionReportId",
                table: "InspectionReportPersons",
                column: "InspectionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReportPersons_PersonId",
                table: "InspectionReportPersons",
                column: "PersonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PersonAttachments_AttachmentId",
                table: "PersonAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAttachments_PersonId",
                table: "PersonAttachments",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationInspectors_Persons_InspectorId",
                table: "CirculationInspectors",
                column: "InspectorId",
                principalTable: "Persons",
                principalColumn: "Id");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CirculationInspectors_Persons_InspectorId",
                table: "CirculationInspectors");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionReports_Contacts_CreatorContactId",
                table: "InspectionReports");


            migrationBuilder.DropTable(
                name: "InspectionReportPersons");

            migrationBuilder.DropTable(
                name: "InspectorOrders");

            migrationBuilder.DropTable(
                name: "InspectorProjects");

            migrationBuilder.DropTable(
                name: "PersonAttachments");
                     

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persons",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "InspectorType",
                table: "InspectionReportNumber");

            migrationBuilder.DropColumn(
                name: "ReportNumber",
                table: "InspectionReportNumber");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Persons");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "Inspectors");

            migrationBuilder.RenameColumn(
                name: "CreatorContactId",
                table: "InspectionReports",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Inspectors",
                newName: "InspectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Persons_InspectionReportId",
                table: "Inspectors",
                newName: "IX_Inspectors_InspectionReportId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inspectors",
                table: "Inspectors",
                column: "InspectorId");

            migrationBuilder.CreateTable(
                name: "InspectorAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTypes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectorAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectorAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectorAttachments_Inspectors_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Inspectors",
                        principalColumn: "InspectorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectorAttachments_AttachmentId",
                table: "InspectorAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectorAttachments_InspectorId",
                table: "InspectorAttachments",
                column: "InspectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationInspectors_Inspectors_InspectorId",
                table: "CirculationInspectors",
                column: "InspectorId",
                principalTable: "Inspectors",
                principalColumn: "InspectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspectors_InspectionReports_InspectionReportId",
                table: "Inspectors",
                column: "InspectionReportId",
                principalTable: "InspectionReports",
                principalColumn: "InspectionReportId");
        }
    }
}

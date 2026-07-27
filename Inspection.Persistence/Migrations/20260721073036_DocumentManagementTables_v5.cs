using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentManagementTables_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionReportPersons_Persons_PersonId",
            //    table: "InspectionReportPersons");

            //migrationBuilder.DropTable(
            //    name: "PersonAttachments");

            //migrationBuilder.DropTable(
            //    name: "Persons");

            //migrationBuilder.DropIndex(
            //    name: "IX_InspectionReportPersons_PersonId",
            //    table: "InspectionReportPersons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Persons",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Persons", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PersonAttachments",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PersonAttachments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PersonAttachments_Attachments_AttachmentId",
            //            column: x => x.AttachmentId,
            //            principalTable: "Attachments",
            //            principalColumn: "AttachmentId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_PersonAttachments_Persons_PersonId",
            //            column: x => x.PersonId,
            //            principalTable: "Persons",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_InspectionReportPersons_PersonId",
            //    table: "InspectionReportPersons",
            //    column: "PersonId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PersonAttachments_AttachmentId",
            //    table: "PersonAttachments",
            //    column: "AttachmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PersonAttachments_PersonId",
            //    table: "PersonAttachments",
            //    column: "PersonId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InspectionReportPersons_Persons_PersonId",
            //    table: "InspectionReportPersons",
            //    column: "PersonId",
            //    principalTable: "Persons",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}

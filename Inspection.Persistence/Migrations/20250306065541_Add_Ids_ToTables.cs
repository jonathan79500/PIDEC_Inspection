using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Ids_ToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "OrderMCContactPersons",
    columns: table => new
    {
        OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        MCContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
        ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_OrderMCContactPersons", x => new { x.OrderId, x.MCContactPersonId });
        table.ForeignKey(
            name: "FK_OrderMCContactPersons_Orders_OrderId",
            column: x => x.OrderId,
            principalTable: "Orders",
            principalColumn: "OrderId",
            onDelete: ReferentialAction.Restrict);
        table.ForeignKey(
            name: "FK_OrderMCContactPersons_MCContactPersons_MCContactPersonId",
            column: x => x.MCContactPersonId,
            principalTable: "MCContactPersons",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMCContactPersons_MCContactPersonId",
                table: "OrderMCContactPersons",
                 column: "MCContactPersonId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProjectOwners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProjectOwners",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProjectOwners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ProjectOwners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ProjectOwners",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProjectMCs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProjectMCs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProjectMCs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ProjectMCs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ProjectMCs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "OrderQCContactPersons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrderQCContactPersons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderQCContactPersons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "OrderQCContactPersons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrderQCContactPersons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

          

            //migrationBuilder.AddColumn<string>(
            //    name: "CreatedBy",
            //    table: "OrderMCContactPersons",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedDate",
            //    table: "OrderMCContactPersons",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<Guid>(
            //    name: "Id",
            //    table: "OrderMCContactPersons",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            //migrationBuilder.AddColumn<string>(
            //    name: "ModifiedBy",
            //    table: "OrderMCContactPersons",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ModifiedDate",
            //    table: "OrderMCContactPersons",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "OrderInspectionSubjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrderInspectionSubjects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderInspectionSubjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "OrderInspectionSubjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrderInspectionSubjects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProjectOwners");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProjectOwners");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectOwners");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ProjectOwners");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ProjectOwners");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProjectMCs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProjectMCs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectMCs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ProjectMCs");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ProjectMCs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OrderQCContactPersons");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderQCContactPersons");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderQCContactPersons");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "OrderQCContactPersons");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "OrderQCContactPersons");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "OrderOwnerContactPersons");

            //migrationBuilder.DropColumn(
            //    name: "CreatedBy",
            //    table: "OrderMCContactPersons");

            //migrationBuilder.DropColumn(
            //    name: "CreatedDate",
            //    table: "OrderMCContactPersons");

            //migrationBuilder.DropColumn(
            //    name: "Id",
            //    table: "OrderMCContactPersons");

            //migrationBuilder.DropColumn(
            //    name: "ModifiedBy",
            //    table: "OrderMCContactPersons");

            //migrationBuilder.DropColumn(
            //    name: "ModifiedDate",
            //    table: "OrderMCContactPersons");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "OrderInspectionSubjects");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeCirculationAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "PDFSize",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "edmsId",
                table: "CirculationAttachmens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "CirculationAttachmens",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CirculationAttachmens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CirculationAttachmens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PDFSize",
                table: "CirculationAttachmens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CirculationAttachmens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "edmsId",
                table: "CirculationAttachmens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

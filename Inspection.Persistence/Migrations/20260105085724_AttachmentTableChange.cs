using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentTableChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<string>(
                name: "DocNo",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocNo",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attachments");

         
        }
    }
}

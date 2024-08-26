using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorDocuments_Orders_OrderId1",
                table: "VendorDocuments");

            migrationBuilder.DropIndex(
                name: "IX_VendorDocuments_OrderId1",
                table: "VendorDocuments");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "VendorDocuments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId1",
                table: "VendorDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorDocuments_OrderId1",
                table: "VendorDocuments",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorDocuments_Orders_OrderId1",
                table: "VendorDocuments",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}

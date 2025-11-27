using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeAttachmentConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CirculationAttachmens_Circulations_CirculationId",
                table: "CirculationAttachmens");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Attachments",
                newName: "AttachmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CirculationAttachmens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationAttachmens_Attachments_AttachmentId",
                table: "CirculationAttachmens",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "AttachmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationAttachmens_Circulations_CirculationId",
                table: "CirculationAttachmens",
                column: "CirculationId",
                principalTable: "Circulations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CirculationAttachmens_Attachments_AttachmentId",
                table: "CirculationAttachmens");

            migrationBuilder.DropForeignKey(
                name: "FK_CirculationAttachmens_Circulations_CirculationId",
                table: "CirculationAttachmens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CirculationAttachmens");

            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                table: "Attachments",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CirculationAttachmens_Circulations_CirculationId",
                table: "CirculationAttachmens",
                column: "CirculationId",
                principalTable: "Circulations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

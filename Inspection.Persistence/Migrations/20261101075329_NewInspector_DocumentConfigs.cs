using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewInspector_DocumentConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "InspectorSkills",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Inspectors",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Inspectors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Inspectors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Inspectors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Inspectors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Inspectors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Inspectors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QCApprovalState",
                table: "Inspectors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "Inspectors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InspectionSkills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InspectionSkills",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentTypeName",
                table: "DocumentTypes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AllowedExtensions",
                table: "DocumentTypes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Documents",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Documents",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Documents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Documents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentTypeVisibilities",
                table: "DocumentTypeVisibilities",
                columns: new[] { "DocumentTypeId", "ContactTypeCode" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentLinks",
                table: "DocumentLinks",
                columns: new[] { "DocumentId", "OwnerType", "OwnerId" });

            migrationBuilder.InsertData(
                table: "InspectionSkills",
                columns: new[] { "SkillId", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-000000000001"), "VT", true, "Visual Testing" },
                    { new Guid("22222222-2222-2222-2222-000000000002"), "RT", true, "Radiographic Testing" },
                    { new Guid("22222222-2222-2222-2222-000000000003"), "UT", true, "Ultrasonic Testing" },
                    { new Guid("22222222-2222-2222-2222-000000000004"), "MT", true, "Magnetic Particle Testing" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectorSkills_InspectorId_SkillId",
                table: "InspectorSkills",
                columns: new[] { "InspectorId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectorSkills_SkillId",
                table: "InspectorSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSkills_Code",
                table: "InspectionSkills",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_DocumentTypeCode",
                table: "DocumentTypes",
                column: "DocumentTypeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLinks_OwnerType_OwnerId",
                table: "DocumentLinks",
                columns: new[] { "OwnerType", "OwnerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentLinks_Documents_DocumentId",
                table: "DocumentLinks",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "DocumentTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTypeVisibilities_DocumentTypes_DocumentTypeId",
                table: "DocumentTypeVisibilities",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "DocumentTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectorSkills_InspectionSkills_SkillId",
                table: "InspectorSkills",
                column: "SkillId",
                principalTable: "InspectionSkills",
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectorSkills_Inspectors_InspectorId",
                table: "InspectorSkills",
                column: "InspectorId",
                principalTable: "Inspectors",
                principalColumn: "InspectorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentLinks_Documents_DocumentId",
                table: "DocumentLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTypeVisibilities_DocumentTypes_DocumentTypeId",
                table: "DocumentTypeVisibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectorSkills_InspectionSkills_SkillId",
                table: "InspectorSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectorSkills_Inspectors_InspectorId",
                table: "InspectorSkills");

            migrationBuilder.DropIndex(
                name: "IX_InspectorSkills_InspectorId_SkillId",
                table: "InspectorSkills");

            migrationBuilder.DropIndex(
                name: "IX_InspectorSkills_SkillId",
                table: "InspectorSkills");

            migrationBuilder.DropIndex(
                name: "IX_InspectionSkills_Code",
                table: "InspectionSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentTypeVisibilities",
                table: "DocumentTypeVisibilities");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTypes_DocumentTypeCode",
                table: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentLinks",
                table: "DocumentLinks");

            migrationBuilder.DropIndex(
                name: "IX_DocumentLinks_OwnerType_OwnerId",
                table: "DocumentLinks");

            migrationBuilder.DeleteData(
                table: "InspectionSkills",
                keyColumn: "SkillId",
                keyValue: new Guid("22222222-2222-2222-2222-000000000001"));

            migrationBuilder.DeleteData(
                table: "InspectionSkills",
                keyColumn: "SkillId",
                keyValue: new Guid("22222222-2222-2222-2222-000000000002"));

            migrationBuilder.DeleteData(
                table: "InspectionSkills",
                keyColumn: "SkillId",
                keyValue: new Guid("22222222-2222-2222-2222-000000000003"));

            migrationBuilder.DeleteData(
                table: "InspectionSkills",
                keyColumn: "SkillId",
                keyValue: new Guid("22222222-2222-2222-2222-000000000004"));

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Inspectors");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Inspectors");

            migrationBuilder.DropColumn(
                name: "QCApprovalState",
                table: "Inspectors");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "Inspectors");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "InspectorSkills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Inspectors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InspectionSkills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InspectionSkills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentTypeName",
                table: "DocumentTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "AllowedExtensions",
                table: "DocumentTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}

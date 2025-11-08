using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class exertLastChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "ProjectOwnerContactPersons",
    columns: table => new
    {
        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false), // Primary key from BaseDomainEntity
        ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false), // Foreign key to Project
        OwnerContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false), // Foreign key to OwnerContactPerson
        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"), // Audit columns
        ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_ProjectOwnerContactPersons", x => x.Id); // Set Id as the primary key
        table.ForeignKey(
            name: "FK_ProjectOwnerContactPersons_Projects_ProjectId",
            column: x => x.ProjectId,
            principalTable: "Projects",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade); // Set foreign key for Project
        table.ForeignKey(
            name: "FK_ProjectOwnerContactPersons_OwnerContactPersons_OwnerContactPersonId",
            column: x => x.OwnerContactPersonId,
            principalTable: "OwnerContactPersons",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade); // Set foreign key for OwnerContactPerson
    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProjectOwnerContactPersons_ProjectId",
            //    table: "ProjectOwnerContactPersons",
            //    column: "ProjectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProjectOwnerContactPersons_OwnerContactPersonId",
            //    table: "ProjectOwnerContactPersons",
            //    column: "OwnerContactPersonId");
            
                        migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectVendors",
                table: "ProjectVendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectOwners",
                table: "ProjectOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectOwnerContactPersons",
                table: "ProjectOwnerContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMCs",
                table: "ProjectMCs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMCContactPersons",
                table: "ProjectMCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderQCContactPersons",
                table: "OrderQCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderOwnerContactPersons",
                table: "OrderOwnerContactPersons");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_OrderMCContactPersons",
            //    table: "OrderMCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderInspectionSubjects",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationTypes",
                table: "InspectionNotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationSubjects",
                table: "InspectionNotificationSubjects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectVendors",
                table: "ProjectVendors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectOwners",
                table: "ProjectOwners",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectOwnerContactPersons",
                table: "ProjectOwnerContactPersons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMCs",
                table: "ProjectMCs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMCContactPersons",
                table: "ProjectMCContactPersons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderQCContactPersons",
                table: "OrderQCContactPersons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderOwnerContactPersons",
                table: "OrderOwnerContactPersons",
                column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_OrderMCContactPersons",
            //    table: "OrderMCContactPersons",
            //    column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderInspectionSubjects",
                table: "OrderInspectionSubjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationTypes",
                table: "InspectionNotificationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationSubjects",
                table: "InspectionNotificationSubjects",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVendors_ProjectId",
                table: "ProjectVendors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwners_ProjectId",
                table: "ProjectOwners",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwnerContactPersons_ProjectId",
                table: "ProjectOwnerContactPersons",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCs_ProjectId",
                table: "ProjectMCs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCContactPersons_ProjectId",
                table: "ProjectMCContactPersons",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderQCContactPersons_OrderId",
                table: "OrderQCContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOwnerContactPersons_OrderId",
                table: "OrderOwnerContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMCContactPersons_OrderId",
                table: "OrderMCContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInspectionSubjects_OrderId",
                table: "OrderInspectionSubjects",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotificationTypes_InspectionNotificationId",
                table: "InspectionNotificationTypes",
                column: "InspectionNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotificationSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects",
                column: "InspectionSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectVendors",
                table: "ProjectVendors");

            migrationBuilder.DropIndex(
                name: "IX_ProjectVendors_ProjectId",
                table: "ProjectVendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectOwners",
                table: "ProjectOwners");

            migrationBuilder.DropIndex(
                name: "IX_ProjectOwners_ProjectId",
                table: "ProjectOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectOwnerContactPersons",
                table: "ProjectOwnerContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_ProjectOwnerContactPersons_ProjectId",
                table: "ProjectOwnerContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMCs",
                table: "ProjectMCs");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMCs_ProjectId",
                table: "ProjectMCs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMCContactPersons",
                table: "ProjectMCContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMCContactPersons_ProjectId",
                table: "ProjectMCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderQCContactPersons",
                table: "OrderQCContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_OrderQCContactPersons_OrderId",
                table: "OrderQCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderOwnerContactPersons",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_OrderOwnerContactPersons_OrderId",
                table: "OrderOwnerContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderMCContactPersons",
                table: "OrderMCContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_OrderMCContactPersons_OrderId",
                table: "OrderMCContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderInspectionSubjects",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropIndex(
                name: "IX_OrderInspectionSubjects_OrderId",
                table: "OrderInspectionSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationTypes",
                table: "InspectionNotificationTypes");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotificationTypes_InspectionNotificationId",
                table: "InspectionNotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationSubjects",
                table: "InspectionNotificationSubjects");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotificationSubjects_InspectionSubjectId",
                table: "InspectionNotificationSubjects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectVendors",
                table: "ProjectVendors",
                columns: new[] { "ProjectId", "VendorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectOwners",
                table: "ProjectOwners",
                columns: new[] { "ProjectId", "OwnerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectOwnerContactPersons",
                table: "ProjectOwnerContactPersons",
                columns: new[] { "ProjectId", "OwnerContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMCs",
                table: "ProjectMCs",
                columns: new[] { "ProjectId", "MCId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMCContactPersons",
                table: "ProjectMCContactPersons",
                columns: new[] { "ProjectId", "MCContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderQCContactPersons",
                table: "OrderQCContactPersons",
                columns: new[] { "OrderId", "QCContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderOwnerContactPersons",
                table: "OrderOwnerContactPersons",
                columns: new[] { "OrderId", "OwnerContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderMCContactPersons",
                table: "OrderMCContactPersons",
                columns: new[] { "OrderId", "MCContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderInspectionSubjects",
                table: "OrderInspectionSubjects",
                columns: new[] { "OrderId", "InspectionSubjectId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationTypes",
                table: "InspectionNotificationTypes",
                columns: new[] { "InspectionNotificationId", "InspectionTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationSubjects",
                table: "InspectionNotificationSubjects",
                columns: new[] { "InspectionSubjectId", "InspectionNotificationId" });
        }
    }
}

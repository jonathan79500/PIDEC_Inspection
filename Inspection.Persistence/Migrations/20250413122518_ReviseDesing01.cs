using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspection.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReviseDesing01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_ActionType_ActionTypeId",
                table: "Circulations");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_MCContactPersons_MCContactPersonContactId",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_OwnerContactPersons_OwnerContactPersonContactId",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InspectionNotifications_VendorContactPersons_VendorContactPersonContactId",
            //    table: "InspectionNotifications");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Orders_VendorContactPersons_VendorContactPersonContactId",
            //    table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderMCContactPersons");

            migrationBuilder.DropTable(
                name: "OrderOwnerContactPersons");

            migrationBuilder.DropTable(
                name: "OrderQCContactPersons");

            migrationBuilder.DropTable(
                name: "ProjectMCContactPersons");

            migrationBuilder.DropTable(
                name: "ProjectMCs");

            migrationBuilder.DropTable(
                name: "ProjectOwnerContactPersons");

            migrationBuilder.DropTable(
                name: "ProjectOwners");

            migrationBuilder.DropTable(
                name: "ProjectVendors");

            migrationBuilder.DropTable(
                name: "SupplierContactPersons");

            migrationBuilder.DropTable(
                name: "VendorContactPersons");

            migrationBuilder.DropTable(
                name: "QCContactPersons");

            migrationBuilder.DropTable(
                name: "MCContactPersons");

            migrationBuilder.DropTable(
                name: "OwnerContactPersons");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "QCMember");

            migrationBuilder.DropTable(
                name: "MC");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Orders_VendorContactPersonContactId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_MCContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_OwnerContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotifications_VendorContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationContacts",
                table: "InspectionNotificationContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActionType",
                table: "ActionType");

            migrationBuilder.DropColumn(
                name: "ShowForOwner",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VendorContactPersonContactId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MCContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "OwnerContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "VendorContactPersonContactId",
                table: "InspectionNotifications");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "InspectionNotificationContacts");

            migrationBuilder.RenameTable(
                name: "ActionType",
                newName: "ActionTypes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Projects",
                newName: "ProjectId");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationContacts",
                table: "InspectionNotificationContacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActionTypes",
                table: "ActionTypes",
                column: "ActionTypeId");

            migrationBuilder.CreateTable(
                name: "OrderContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderContacts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectContacts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotificationContacts_InspectionNotificationId",
                table: "InspectionNotificationContacts",
                column: "InspectionNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderContacts_ContactId",
                table: "OrderContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderContacts_OrderId",
                table: "OrderContacts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContacts_ContactId",
                table: "ProjectContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContacts_ProjectId",
                table: "ProjectContacts",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_ActionTypes_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "ActionTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Circulations_ActionTypes_ActionTypeId",
                table: "Circulations");

            migrationBuilder.DropTable(
                name: "OrderContacts");

            migrationBuilder.DropTable(
                name: "ProjectContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionNotificationContacts",
                table: "InspectionNotificationContacts");

            migrationBuilder.DropIndex(
                name: "IX_InspectionNotificationContacts_InspectionNotificationId",
                table: "InspectionNotificationContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActionTypes",
                table: "ActionTypes");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "ActionTypes",
                newName: "ActionType");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Projects",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "ShowForOwner",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorContactPersonContactId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MCContactPersonContactId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerContactPersonContactId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorContactPersonContactId",
                table: "InspectionNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "InspectionNotificationContacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionNotificationContacts",
                table: "InspectionNotificationContacts",
                columns: new[] { "InspectionNotificationId", "ContactId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActionType",
                table: "ActionType",
                column: "ActionTypeId");

            migrationBuilder.CreateTable(
                name: "MC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QCMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCMember", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierContactPersons", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_SupplierContactPersons_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MCId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCContactPersons", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_MCContactPersons_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MCContactPersons_MC_MCId",
                        column: x => x.MCId,
                        principalTable: "MC",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMCs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MCId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMCs_MC_MCId",
                        column: x => x.MCId,
                        principalTable: "MC",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMCs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnerContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerContactPersons", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_OwnerContactPersons_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnerContactPersons_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectOwners_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectOwners_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QCContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QCMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCContactPersons", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_QCContactPersons_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QCContactPersons_QCMember_QCMemberId",
                        column: x => x.QCMemberId,
                        principalTable: "QCMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVendors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectVendors_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorContactPersons",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorContactPersons", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_VendorContactPersons_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorContactPersons_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderMCContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MCContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMCContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMCContactPersons_MCContactPersons_MCContactPersonId",
                        column: x => x.MCContactPersonId,
                        principalTable: "MCContactPersons",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderMCContactPersons_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMCContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MCContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMCContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMCContactPersons_MCContactPersons_MCContactPersonId",
                        column: x => x.MCContactPersonId,
                        principalTable: "MCContactPersons",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMCContactPersons_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderOwnerContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOwnerContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderOwnerContactPersons_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOwnerContactPersons_OwnerContactPersons_OwnerContactPersonId",
                        column: x => x.OwnerContactPersonId,
                        principalTable: "OwnerContactPersons",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectOwnerContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectOwnerContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectOwnerContactPersons_OwnerContactPersons_OwnerContactPersonId",
                        column: x => x.OwnerContactPersonId,
                        principalTable: "OwnerContactPersons",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectOwnerContactPersons_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderQCContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QCContactPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderQCContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderQCContactPersons_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderQCContactPersons_QCContactPersons_QCContactPersonId",
                        column: x => x.QCContactPersonId,
                        principalTable: "QCContactPersons",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VendorContactPersonContactId",
                table: "Orders",
                column: "VendorContactPersonContactId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_MCContactPersonContactId",
                table: "InspectionNotifications",
                column: "MCContactPersonContactId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_OwnerContactPersonContactId",
                table: "InspectionNotifications",
                column: "OwnerContactPersonContactId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionNotifications_VendorContactPersonContactId",
                table: "InspectionNotifications",
                column: "VendorContactPersonContactId");

            migrationBuilder.CreateIndex(
                name: "IX_MCContactPersons_MCId",
                table: "MCContactPersons",
                column: "MCId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMCContactPersons_MCContactPersonId",
                table: "OrderMCContactPersons",
                column: "MCContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMCContactPersons_OrderId",
                table: "OrderMCContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOwnerContactPersons_OrderId",
                table: "OrderOwnerContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOwnerContactPersons_OwnerContactPersonId",
                table: "OrderOwnerContactPersons",
                column: "OwnerContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderQCContactPersons_OrderId",
                table: "OrderQCContactPersons",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderQCContactPersons_QCContactPersonId",
                table: "OrderQCContactPersons",
                column: "QCContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerContactPersons_OwnerId",
                table: "OwnerContactPersons",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCContactPersons_MCContactPersonId",
                table: "ProjectMCContactPersons",
                column: "MCContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCContactPersons_ProjectId",
                table: "ProjectMCContactPersons",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCs_MCId",
                table: "ProjectMCs",
                column: "MCId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMCs_ProjectId",
                table: "ProjectMCs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwnerContactPersons_OwnerContactPersonId",
                table: "ProjectOwnerContactPersons",
                column: "OwnerContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwnerContactPersons_ProjectId",
                table: "ProjectOwnerContactPersons",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwners_OwnerId",
                table: "ProjectOwners",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectOwners_ProjectId",
                table: "ProjectOwners",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVendors_ProjectId",
                table: "ProjectVendors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVendors_VendorId",
                table: "ProjectVendors",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_QCContactPersons_QCMemberId",
                table: "QCContactPersons",
                column: "QCMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContactPersons_VendorId",
                table: "VendorContactPersons",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_AddressId",
                table: "Vendors",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Circulations_ActionType_ActionTypeId",
                table: "Circulations",
                column: "ActionTypeId",
                principalTable: "ActionType",
                principalColumn: "ActionTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_MCContactPersons_MCContactPersonContactId",
                table: "InspectionNotifications",
                column: "MCContactPersonContactId",
                principalTable: "MCContactPersons",
                principalColumn: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_OwnerContactPersons_OwnerContactPersonContactId",
                table: "InspectionNotifications",
                column: "OwnerContactPersonContactId",
                principalTable: "OwnerContactPersons",
                principalColumn: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionNotifications_VendorContactPersons_VendorContactPersonContactId",
                table: "InspectionNotifications",
                column: "VendorContactPersonContactId",
                principalTable: "VendorContactPersons",
                principalColumn: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_VendorContactPersons_VendorContactPersonContactId",
                table: "Orders",
                column: "VendorContactPersonContactId",
                principalTable: "VendorContactPersons",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

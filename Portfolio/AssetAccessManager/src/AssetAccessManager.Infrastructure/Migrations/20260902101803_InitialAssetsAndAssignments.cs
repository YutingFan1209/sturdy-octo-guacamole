using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetAccessManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAssetsAndAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReturnedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReturnedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetTag", "Model", "SerialNumber", "Status", "Version" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "LT-1001", "MacBook Pro 14", "SN-APPLE-001", "Available", 0L },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "LT-1002", "Dell Latitude 7450", "SN-DELL-002", "Available", 0L },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "LT-1003", "ThinkPad T14", "SN-LENOVO-003", "Available", 0L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTag",
                table: "Assets",
                column: "AssetTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status_AssetTag",
                table: "Assets",
                columns: new[] { "Status", "AssetTag" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssetId",
                table: "Assignments",
                column: "AssetId",
                unique: true,
                filter: "[ReturnedAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeOperations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialEquipmentRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Item = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestTransitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTransitions_EquipmentRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "EquipmentRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRequests_EmployeeId_Status_UpdatedAt",
                table: "EquipmentRequests",
                columns: new[] { "EmployeeId", "Status", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTransitions_RequestId_OccurredAt",
                table: "RequestTransitions",
                columns: new[] { "RequestId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestTransitions");

            migrationBuilder.DropTable(
                name: "EquipmentRequests");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class Renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.CreateTable(
                name: "UserPricingSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Marketplace = table.Column<int>(type: "INTEGER", nullable: false),
                    NonFoilMetric = table.Column<int>(type: "INTEGER", nullable: false),
                    FoilMetric = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPricingSettings", x => x.Id);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\r\n                    )\r\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPricingSettings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                    (\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\n                    )\n                    ");
        }
    }
}

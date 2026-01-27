using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddedBoughForIsStamped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddColumn<decimal>(
                name: "BoughtFor",
                table: "Allocations",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStamped",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                    (\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\n                    )\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "BoughtFor",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "IsStamped",
                table: "Allocations");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\r\n                    )\r\n                    ");
        }
    }
}

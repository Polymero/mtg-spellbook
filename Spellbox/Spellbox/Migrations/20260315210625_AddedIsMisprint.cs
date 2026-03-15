using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsMisprint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddColumn<bool>(
                name: "IsMisprint",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                    (\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\n                    )\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "IsMisprint",
                table: "Allocations");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\r\n                    )\r\n                    ");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class OptIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Snapshots_DeckId",
                table: "Snapshots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_DeckId",
                table: "Snapshots",
                column: "DeckId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                    (\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\n                    )\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Snapshots_DeckId",
                table: "Snapshots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_DeckId",
                table: "Snapshots",
                column: "DeckId",
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\r\n                    )\r\n                    ");
        }
    }
}

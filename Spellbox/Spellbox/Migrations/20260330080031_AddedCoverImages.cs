using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoverImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddColumn<string>(
                name: "CoverImages",
                table: "Decks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoverImages",
                table: "Binders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
                name: "CoverImages",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "CoverImages",
                table: "Binders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\r\n                    )\r\n                    ");
        }
    }
}

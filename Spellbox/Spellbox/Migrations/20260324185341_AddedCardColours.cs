using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddedCardColours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AlterColumn<int>(
                name: "ColorIdentity",
                table: "Decks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\r\n                    )\r\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AlterColumn<string>(
                name: "ColorIdentity",
                table: "Decks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                    (\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\n                    )\n                    ");
        }
    }
}

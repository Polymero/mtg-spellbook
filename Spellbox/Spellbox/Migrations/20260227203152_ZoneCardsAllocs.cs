using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class ZoneCardsAllocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND ZoneId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND ZoneId IS NOT NULL)\r\n                    )\r\n                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND DeckId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND DeckId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND DeckId IS NOT NULL)\r\n                    )\r\n                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

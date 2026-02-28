using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class DeckContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Snapshots_SnapshotId",
                table: "Allocations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.RenameColumn(
                name: "SnapshotId",
                table: "Allocations",
                newName: "ZoneId");

            migrationBuilder.RenameIndex(
                name: "IX_Allocations_SnapshotId",
                table: "Allocations",
                newName: "IX_Allocations_ZoneId");

            migrationBuilder.AddColumn<Guid>(
                name: "DeckId",
                table: "Allocations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_DeckId",
                table: "Allocations",
                column: "DeckId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND DeckId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND DeckId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND DeckId IS NOT NULL)\r\n                    )\r\n                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Decks_DeckId",
                table: "Allocations",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Zones_ZoneId",
                table: "Allocations",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Decks_DeckId",
                table: "Allocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Zones_ZoneId",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_DeckId",
                table: "Allocations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "Allocations");

            migrationBuilder.RenameColumn(
                name: "ZoneId",
                table: "Allocations",
                newName: "SnapshotId");

            migrationBuilder.RenameIndex(
                name: "IX_Allocations_ZoneId",
                table: "Allocations",
                newName: "IX_Allocations_SnapshotId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\r\n                    )\r\n                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Snapshots_SnapshotId",
                table: "Allocations",
                column: "SnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

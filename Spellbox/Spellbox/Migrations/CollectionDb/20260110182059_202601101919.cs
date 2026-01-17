using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CollectionDb
{
    /// <inheritdoc />
    public partial class _202601101919 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "Altered",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "Finish",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "CollectionCards");

            migrationBuilder.DropColumn(
                name: "Signed",
                table: "CollectionCards");

            migrationBuilder.AddColumn<string>(
                name: "DeckName",
                table: "Snapshots",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Snapshots",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Condition",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DeckSnapshotId",
                table: "Allocations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Finish",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAltered",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSigned",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Allocations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_DeckSnapshotId",
                table: "Allocations",
                column: "DeckSnapshotId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\r\n                    (\r\n                        (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\r\n                        (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\r\n                    )\r\n                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Snapshots_DeckSnapshotId",
                table: "Allocations",
                column: "DeckSnapshotId",
                principalTable: "Snapshots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Snapshots_DeckSnapshotId",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_DeckSnapshotId",
                table: "Allocations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "DeckName",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "DeckSnapshotId",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "Finish",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "IsAltered",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "IsSigned",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Allocations");

            migrationBuilder.AddColumn<string>(
                name: "Altered",
                table: "CollectionCards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "CollectionCards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Finish",
                table: "CollectionCards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "CollectionCards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Signed",
                table: "CollectionCards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollectionAllocation_AllocationIndex",
                table: "Allocations",
                sql: "\n                (\n                    (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\n                    (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\n                    (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\n                )\n                ");
        }
    }
}

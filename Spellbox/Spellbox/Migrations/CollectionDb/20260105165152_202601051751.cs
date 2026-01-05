using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CollectionDb
{
    /// <inheritdoc />
    public partial class _202601051751 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Binders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Binders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Finish = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Condition = table.Column<string>(type: "TEXT", nullable: true),
                    Altered = table.Column<string>(type: "TEXT", nullable: true),
                    Signed = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeckId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snapshots_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CollectionCardId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AllocationIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    BinderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SnapshotId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AllocatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CollectionBinderId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.Id);
                    table.CheckConstraint("CK_CollectionAllocation_AllocationIndex", "\n                (\n                    (AllocationIndex = 0 AND BinderId IS NULL AND SnapshotId IS NULL) OR\n                    (AllocationIndex = 1 AND BinderId IS NOT NULL AND SnapshotId IS NULL) OR\n                    (AllocationIndex = 2 AND BinderId IS NULL AND SnapshotId IS NOT NULL)\n                )\n                ");
                    table.ForeignKey(
                        name: "FK_Allocations_Binders_BinderId",
                        column: x => x.BinderId,
                        principalTable: "Binders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Allocations_Binders_CollectionBinderId",
                        column: x => x.CollectionBinderId,
                        principalTable: "Binders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Allocations_CollectionCards_CollectionCardId",
                        column: x => x.CollectionCardId,
                        principalTable: "CollectionCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SnapshotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ZoneType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zones_Snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeckCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ZoneId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckCards_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_BinderId",
                table: "Allocations",
                column: "BinderId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_CollectionBinderId",
                table: "Allocations",
                column: "CollectionBinderId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_SnapshotId",
                table: "Allocations",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCards_OracleId_VariantId",
                table: "CollectionCards",
                columns: new[] { "OracleId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeckCards_ZoneId_OracleId_VariantId",
                table: "DeckCards",
                columns: new[] { "ZoneId", "OracleId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_DeckId",
                table: "Snapshots",
                column: "DeckId",
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_SnapshotId_ZoneType",
                table: "Zones",
                columns: new[] { "SnapshotId", "ZoneType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "DeckCards");

            migrationBuilder.DropTable(
                name: "Binders");

            migrationBuilder.DropTable(
                name: "CollectionCards");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Snapshots");

            migrationBuilder.DropTable(
                name: "Decks");
        }
    }
}

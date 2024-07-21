using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Binders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<string>(type: "TEXT", nullable: true),
                    Updated = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Binders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<string>(type: "TEXT", nullable: true),
                    Updated = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<string>(type: "TEXT", nullable: true),
                    DeckId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snapshot_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<string>(type: "TEXT", nullable: true),
                    SetName = table.Column<string>(type: "TEXT", nullable: true),
                    SetId = table.Column<string>(type: "TEXT", nullable: true),
                    CollectorNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Finish = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    BinderId = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotId = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotId2 = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotId3 = table.Column<int>(type: "INTEGER", nullable: true),
                    SnapshotId4 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_Binders_BinderId",
                        column: x => x.BinderId,
                        principalTable: "Binders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Snapshot_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "Snapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Snapshot_SnapshotId1",
                        column: x => x.SnapshotId1,
                        principalTable: "Snapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Snapshot_SnapshotId2",
                        column: x => x.SnapshotId2,
                        principalTable: "Snapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Snapshot_SnapshotId3",
                        column: x => x.SnapshotId3,
                        principalTable: "Snapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Snapshot_SnapshotId4",
                        column: x => x.SnapshotId4,
                        principalTable: "Snapshot",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_BinderId",
                table: "Card",
                column: "BinderId");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SnapshotId",
                table: "Card",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SnapshotId1",
                table: "Card",
                column: "SnapshotId1");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SnapshotId2",
                table: "Card",
                column: "SnapshotId2");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SnapshotId3",
                table: "Card",
                column: "SnapshotId3");

            migrationBuilder.CreateIndex(
                name: "IX_Card_SnapshotId4",
                table: "Card",
                column: "SnapshotId4");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshot_DeckId",
                table: "Snapshot",
                column: "DeckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "Binders");

            migrationBuilder.DropTable(
                name: "Snapshot");

            migrationBuilder.DropTable(
                name: "Decks");
        }
    }
}

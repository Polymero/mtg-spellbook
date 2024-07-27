using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CollectionDb
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
                    Name = table.Column<string>(type: "TEXT", nullable: true),
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
                    Name = table.Column<string>(type: "TEXT", nullable: true),
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
                name: "CollectionSnapshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<string>(type: "TEXT", nullable: true),
                    CollectionDeckId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionSnapshot_Decks_CollectionDeckId",
                        column: x => x.CollectionDeckId,
                        principalTable: "Decks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CollectionCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<string>(type: "TEXT", nullable: true),
                    VariantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CollectionBinderId = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionSnapshotId = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionSnapshotId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionSnapshotId2 = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionSnapshotId3 = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionSnapshotId4 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionCard_Binders_CollectionBinderId",
                        column: x => x.CollectionBinderId,
                        principalTable: "Binders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCard_CollectionSnapshot_CollectionSnapshotId",
                        column: x => x.CollectionSnapshotId,
                        principalTable: "CollectionSnapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCard_CollectionSnapshot_CollectionSnapshotId1",
                        column: x => x.CollectionSnapshotId1,
                        principalTable: "CollectionSnapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCard_CollectionSnapshot_CollectionSnapshotId2",
                        column: x => x.CollectionSnapshotId2,
                        principalTable: "CollectionSnapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCard_CollectionSnapshot_CollectionSnapshotId3",
                        column: x => x.CollectionSnapshotId3,
                        principalTable: "CollectionSnapshot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionCard_CollectionSnapshot_CollectionSnapshotId4",
                        column: x => x.CollectionSnapshotId4,
                        principalTable: "CollectionSnapshot",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionBinderId",
                table: "CollectionCard",
                column: "CollectionBinderId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionSnapshotId",
                table: "CollectionCard",
                column: "CollectionSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionSnapshotId1",
                table: "CollectionCard",
                column: "CollectionSnapshotId1");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionSnapshotId2",
                table: "CollectionCard",
                column: "CollectionSnapshotId2");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionSnapshotId3",
                table: "CollectionCard",
                column: "CollectionSnapshotId3");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCard_CollectionSnapshotId4",
                table: "CollectionCard",
                column: "CollectionSnapshotId4");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSnapshot_CollectionDeckId",
                table: "CollectionSnapshot",
                column: "CollectionDeckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionCard");

            migrationBuilder.DropTable(
                name: "Binders");

            migrationBuilder.DropTable(
                name: "CollectionSnapshot");

            migrationBuilder.DropTable(
                name: "Decks");
        }
    }
}

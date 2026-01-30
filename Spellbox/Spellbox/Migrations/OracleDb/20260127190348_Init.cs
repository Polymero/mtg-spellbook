using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.OracleDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportCheckpoints",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    CardsProcessed = table.Column<long>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportCheckpoints", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Oracles",
                columns: table => new
                {
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TypeLine = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    CMC = table.Column<decimal>(type: "TEXT", nullable: false),
                    ColorIdentity = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oracles", x => x.OracleId);
                });

            migrationBuilder.CreateTable(
                name: "SyncStates",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncStates", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Faces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ManaCost = table.Column<string>(type: "TEXT", nullable: true),
                    TypeLine = table.Column<string>(type: "TEXT", nullable: false),
                    OracleText = table.Column<string>(type: "TEXT", nullable: true),
                    Power = table.Column<string>(type: "TEXT", nullable: true),
                    Toughness = table.Column<string>(type: "TEXT", nullable: true),
                    Defense = table.Column<string>(type: "TEXT", nullable: true),
                    Loyalty = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faces_Oracles_OracleId",
                        column: x => x.OracleId,
                        principalTable: "Oracles",
                        principalColumn: "OracleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Variants",
                columns: table => new
                {
                    ScryfallId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SearchName = table.Column<string>(type: "TEXT", nullable: false),
                    SetName = table.Column<string>(type: "TEXT", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false),
                    CollNum = table.Column<string>(type: "TEXT", nullable: false),
                    Finishes = table.Column<string>(type: "TEXT", nullable: false),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Released = table.Column<string>(type: "TEXT", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    FlavorTexts = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbs = table.Column<string>(type: "TEXT", nullable: false),
                    Images = table.Column<string>(type: "TEXT", nullable: false),
                    CardMarketProductId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variants", x => x.ScryfallId);
                    table.ForeignKey(
                        name: "FK_Variants_Oracles_OracleId",
                        column: x => x.OracleId,
                        principalTable: "Oracles",
                        principalColumn: "OracleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faces_OracleId",
                table: "Faces",
                column: "OracleId");

            migrationBuilder.CreateIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces",
                columns: new[] { "OracleId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Oracles_Name",
                table: "Oracles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_CardMarketProductId",
                table: "Variants",
                column: "CardMarketProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_OracleId",
                table: "Variants",
                column: "OracleId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_SearchName",
                table: "Variants",
                column: "SearchName");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_SetCode_CollNum",
                table: "Variants",
                columns: new[] { "SetCode", "CollNum" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faces");

            migrationBuilder.DropTable(
                name: "ImportCheckpoints");

            migrationBuilder.DropTable(
                name: "SyncStates");

            migrationBuilder.DropTable(
                name: "Variants");

            migrationBuilder.DropTable(
                name: "Oracles");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class _202601010509_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OracleCards",
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
                    table.PrimaryKey("PK_OracleCards", x => x.OracleId);
                });

            migrationBuilder.CreateTable(
                name: "CardFaces",
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
                    Defense = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardFaces_OracleCards_OracleId",
                        column: x => x.OracleId,
                        principalTable: "OracleCards",
                        principalColumn: "OracleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardVariants",
                columns: table => new
                {
                    ScryfallId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleCardId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SetName = table.Column<string>(type: "TEXT", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false),
                    CollNum = table.Column<string>(type: "TEXT", nullable: false),
                    Finishes = table.Column<string>(type: "TEXT", nullable: false),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Released = table.Column<string>(type: "TEXT", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    Thumbs = table.Column<string>(type: "TEXT", nullable: false),
                    Images = table.Column<string>(type: "TEXT", nullable: false),
                    FlavorTexts = table.Column<string>(type: "TEXT", nullable: false),
                    FlavorName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardVariants", x => x.ScryfallId);
                    table.ForeignKey(
                        name: "FK_CardVariants_OracleCards_OracleCardId",
                        column: x => x.OracleCardId,
                        principalTable: "OracleCards",
                        principalColumn: "OracleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFaces_OracleId_Order",
                table: "CardFaces",
                columns: new[] { "OracleId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardVariants_OracleCardId",
                table: "CardVariants",
                column: "OracleCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardFaces");

            migrationBuilder.DropTable(
                name: "CardVariants");

            migrationBuilder.DropTable(
                name: "OracleCards");
        }
    }
}

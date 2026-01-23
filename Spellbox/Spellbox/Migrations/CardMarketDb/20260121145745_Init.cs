using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CardMarketDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceLow = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceTrend = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceAverage = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceFoilLow = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PriceFoilTrend = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false),
                    CollNum = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Finish = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceCaches_ProductId",
                table: "PriceCaches",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMappings_CardVariantId_Finish_Language",
                table: "ProductMappings",
                columns: new[] { "CardVariantId", "Finish", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMappings_ProductId",
                table: "ProductMappings",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceCaches");

            migrationBuilder.DropTable(
                name: "ProductMappings");
        }
    }
}

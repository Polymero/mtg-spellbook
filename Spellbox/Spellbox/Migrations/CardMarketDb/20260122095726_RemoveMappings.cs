using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CardMarketDb
{
    /// <inheritdoc />
    public partial class RemoveMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMappings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CollNum = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Finish = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMappings", x => x.Id);
                });

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
    }
}

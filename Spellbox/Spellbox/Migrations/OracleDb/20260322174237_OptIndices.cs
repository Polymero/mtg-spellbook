using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.OracleDb
{
    /// <inheritdoc />
    public partial class OptIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Variants_CardMarketProductId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_SearchName",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Variants_CardMarketProductId",
                table: "Variants",
                column: "CardMarketProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Variants_SearchName",
                table: "Variants",
                column: "SearchName");

            migrationBuilder.CreateIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces",
                columns: new[] { "OracleId", "Order" },
                unique: true);
        }
    }
}

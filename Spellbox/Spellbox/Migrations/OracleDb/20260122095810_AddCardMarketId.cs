using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.OracleDb
{
    /// <inheritdoc />
    public partial class AddCardMarketId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardMarketProductId",
                table: "CardVariants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardVariants_CardMarketProductId",
                table: "CardVariants",
                column: "CardMarketProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CardVariants_CardMarketProductId",
                table: "CardVariants");

            migrationBuilder.DropColumn(
                name: "CardMarketProductId",
                table: "CardVariants");
        }
    }
}

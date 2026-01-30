using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CardMarketDb
{
    /// <inheritdoc />
    public partial class Renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceTrend",
                table: "PriceCaches",
                newName: "Trend");

            migrationBuilder.RenameColumn(
                name: "PriceLow",
                table: "PriceCaches",
                newName: "Low");

            migrationBuilder.RenameColumn(
                name: "PriceFoilTrend",
                table: "PriceCaches",
                newName: "FoilTrend");

            migrationBuilder.RenameColumn(
                name: "PriceFoilLow",
                table: "PriceCaches",
                newName: "FoilLow");

            migrationBuilder.RenameColumn(
                name: "PriceAverage",
                table: "PriceCaches",
                newName: "FoilAvg7");

            migrationBuilder.AddColumn<decimal>(
                name: "Avg",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Avg1",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Avg30",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Avg7",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FoilAvg",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FoilAvg1",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FoilAvg30",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avg",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "Avg1",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "Avg30",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "Avg7",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "FoilAvg",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "FoilAvg1",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "FoilAvg30",
                table: "PriceCaches");

            migrationBuilder.RenameColumn(
                name: "Trend",
                table: "PriceCaches",
                newName: "PriceTrend");

            migrationBuilder.RenameColumn(
                name: "Low",
                table: "PriceCaches",
                newName: "PriceLow");

            migrationBuilder.RenameColumn(
                name: "FoilTrend",
                table: "PriceCaches",
                newName: "PriceFoilTrend");

            migrationBuilder.RenameColumn(
                name: "FoilLow",
                table: "PriceCaches",
                newName: "PriceFoilLow");

            migrationBuilder.RenameColumn(
                name: "FoilAvg7",
                table: "PriceCaches",
                newName: "PriceAverage");
        }
    }
}

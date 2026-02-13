using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CardMarketDb
{
    /// <inheritdoc />
    public partial class RfColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FoilAvg1",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "FoilAvg30",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "FoilAvg7",
                table: "PriceCaches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FoilAvg1",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FoilAvg30",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FoilAvg7",
                table: "PriceCaches",
                type: "decimal(10,2)",
                nullable: true);
        }
    }
}

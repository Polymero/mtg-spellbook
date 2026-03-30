using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Decks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Decks",
                type: "TEXT",
                nullable: true);
        }
    }
}

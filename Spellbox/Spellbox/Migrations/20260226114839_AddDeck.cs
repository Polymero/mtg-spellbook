using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddDeck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "DeckName",
                table: "Snapshots");

            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Decks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots");

            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Decks");

            migrationBuilder.AddColumn<string>(
                name: "DeckName",
                table: "Snapshots",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckCards_Zones_ZoneId",
                table: "DeckCards",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snapshots_Decks_DeckId",
                table: "Snapshots",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

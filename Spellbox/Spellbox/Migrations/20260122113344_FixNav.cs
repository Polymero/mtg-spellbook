using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class FixNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId",
                principalTable: "CollectionCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId",
                principalTable: "CollectionCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

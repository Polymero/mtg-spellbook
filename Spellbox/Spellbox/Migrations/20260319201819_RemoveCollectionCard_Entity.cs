using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCollectionCard_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
PRAGMA foreign_keys = OFF;
PRAGMA defer_foreign_keys = ON;
");

            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations");

            migrationBuilder.DropTable(
                name: "CollectionCards");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_CollectionCardId",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "CollectionCardId",
                table: "Allocations");

            migrationBuilder.Sql(
@"
PRAGMA foreign_keys = ON;
PRAGMA defer_foreign_keys = OFF;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollectionCardId",
                table: "Allocations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollectionCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OracleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    VariantId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCards_OracleId_VariantId",
                table: "CollectionCards",
                columns: new[] { "OracleId", "VariantId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId",
                principalTable: "CollectionCards",
                principalColumn: "Id");
        }
    }
}

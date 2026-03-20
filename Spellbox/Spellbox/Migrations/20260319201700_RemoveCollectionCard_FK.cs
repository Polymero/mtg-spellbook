using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCollectionCard_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations");

            migrationBuilder.AlterColumn<Guid>(
                name: "CollectionCardId",
                table: "Allocations",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId",
                principalTable: "CollectionCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations");

            migrationBuilder.AlterColumn<Guid>(
                name: "CollectionCardId",
                table: "Allocations",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_CollectionCards_CollectionCardId",
                table: "Allocations",
                column: "CollectionCardId",
                principalTable: "CollectionCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.CardMarketDb
{
    /// <inheritdoc />
    public partial class RemoveGuidId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceCaches",
                table: "PriceCaches");

            migrationBuilder.DropIndex(
                name: "IX_PriceCaches_ProductId",
                table: "PriceCaches");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PriceCaches");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PriceCaches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceCaches",
                table: "PriceCaches",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceCaches",
                table: "PriceCaches");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PriceCaches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PriceCaches",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceCaches",
                table: "PriceCaches",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PriceCaches_ProductId",
                table: "PriceCaches",
                column: "ProductId",
                unique: true);
        }
    }
}

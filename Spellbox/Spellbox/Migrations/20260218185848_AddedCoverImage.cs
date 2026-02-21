using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Binders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Binders");
        }
    }
}

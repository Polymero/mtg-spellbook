using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.OracleDb
{
    /// <inheritdoc />
    public partial class ChangedFaceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces");

            migrationBuilder.CreateIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces",
                columns: new[] { "OracleId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces");

            migrationBuilder.CreateIndex(
                name: "IX_Faces_OracleId_Order",
                table: "Faces",
                columns: new[] { "OracleId", "Order" });
        }
    }
}

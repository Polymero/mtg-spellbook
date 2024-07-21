using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations.OracleDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Legalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Standard = table.Column<string>(type: "TEXT", nullable: true),
                    Future = table.Column<string>(type: "TEXT", nullable: true),
                    Historic = table.Column<string>(type: "TEXT", nullable: true),
                    Timeless = table.Column<string>(type: "TEXT", nullable: true),
                    Gladiator = table.Column<string>(type: "TEXT", nullable: true),
                    Pioneer = table.Column<string>(type: "TEXT", nullable: true),
                    Explorer = table.Column<string>(type: "TEXT", nullable: true),
                    Modern = table.Column<string>(type: "TEXT", nullable: true),
                    Legacy = table.Column<string>(type: "TEXT", nullable: true),
                    Pauper = table.Column<string>(type: "TEXT", nullable: true),
                    Vintage = table.Column<string>(type: "TEXT", nullable: true),
                    Penny = table.Column<string>(type: "TEXT", nullable: true),
                    Commander = table.Column<string>(type: "TEXT", nullable: true),
                    Oathbreaker = table.Column<string>(type: "TEXT", nullable: true),
                    PauperCommander = table.Column<string>(type: "TEXT", nullable: true),
                    Duel = table.Column<string>(type: "TEXT", nullable: true),
                    Oldschool = table.Column<string>(type: "TEXT", nullable: true),
                    Premodern = table.Column<string>(type: "TEXT", nullable: true),
                    Predh = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OracleCards",
                columns: table => new
                {
                    OracleId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TypeLine = table.Column<string>(type: "TEXT", nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true),
                    OracleText = table.Column<string>(type: "TEXT", nullable: true),
                    ManaCost = table.Column<string>(type: "TEXT", nullable: true),
                    CMC = table.Column<decimal>(type: "TEXT", nullable: true),
                    Colors = table.Column<string>(type: "TEXT", nullable: true),
                    ColorIdentity = table.Column<string>(type: "TEXT", nullable: true),
                    LegalitiesId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OracleCards", x => x.OracleId);
                    table.ForeignKey(
                        name: "FK_OracleCards_Legalities_LegalitiesId",
                        column: x => x.LegalitiesId,
                        principalTable: "Legalities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardVariant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetName = table.Column<string>(type: "TEXT", nullable: true),
                    SetCode = table.Column<string>(type: "TEXT", nullable: true),
                    CollNum = table.Column<string>(type: "TEXT", nullable: true),
                    Finishes = table.Column<string>(type: "TEXT", nullable: true),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Released = table.Column<string>(type: "TEXT", nullable: true),
                    Rarity = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorName = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorText = table.Column<string>(type: "TEXT", nullable: true),
                    OracleCardOracleId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardVariant_OracleCards_OracleCardOracleId",
                        column: x => x.OracleCardOracleId,
                        principalTable: "OracleCards",
                        principalColumn: "OracleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardVariant_OracleCardOracleId",
                table: "CardVariant",
                column: "OracleCardOracleId");

            migrationBuilder.CreateIndex(
                name: "IX_OracleCards_LegalitiesId",
                table: "OracleCards",
                column: "LegalitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardVariant");

            migrationBuilder.DropTable(
                name: "OracleCards");

            migrationBuilder.DropTable(
                name: "Legalities");
        }
    }
}

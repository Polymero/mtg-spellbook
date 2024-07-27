using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spellbox.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardLegalities",
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
                    table.PrimaryKey("PK_CardLegalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OracleCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_OracleCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OracleCards_CardLegalities_LegalitiesId",
                        column: x => x.LegalitiesId,
                        principalTable: "CardLegalities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardFace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ManaCost = table.Column<string>(type: "TEXT", nullable: true),
                    TypeLine = table.Column<string>(type: "TEXT", nullable: true),
                    OracleText = table.Column<string>(type: "TEXT", nullable: true),
                    Power = table.Column<string>(type: "TEXT", nullable: true),
                    Toughness = table.Column<string>(type: "TEXT", nullable: true),
                    Defense = table.Column<string>(type: "TEXT", nullable: true),
                    OracleCardId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardFace_OracleCards_OracleCardId",
                        column: x => x.OracleCardId,
                        principalTable: "OracleCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<string>(type: "TEXT", nullable: false),
                    SetName = table.Column<string>(type: "TEXT", nullable: true),
                    SetCode = table.Column<string>(type: "TEXT", nullable: true),
                    CollNum = table.Column<string>(type: "TEXT", nullable: true),
                    Finishes = table.Column<string>(type: "TEXT", nullable: true),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Released = table.Column<string>(type: "TEXT", nullable: true),
                    Rarity = table.Column<string>(type: "TEXT", nullable: true),
                    Thumbs = table.Column<string>(type: "TEXT", nullable: true),
                    Images = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorTexts = table.Column<string>(type: "TEXT", nullable: true),
                    FlavorNames = table.Column<string>(type: "TEXT", nullable: true),
                    OracleCardId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardVariants_OracleCards_OracleCardId",
                        column: x => x.OracleCardId,
                        principalTable: "OracleCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFace_OracleCardId",
                table: "CardFace",
                column: "OracleCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardVariants_OracleCardId",
                table: "CardVariants",
                column: "OracleCardId");

            migrationBuilder.CreateIndex(
                name: "IX_OracleCards_LegalitiesId",
                table: "OracleCards",
                column: "LegalitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardFace");

            migrationBuilder.DropTable(
                name: "CardVariants");

            migrationBuilder.DropTable(
                name: "OracleCards");

            migrationBuilder.DropTable(
                name: "CardLegalities");
        }
    }
}

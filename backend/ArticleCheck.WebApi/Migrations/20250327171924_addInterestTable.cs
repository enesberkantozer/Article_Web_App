using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addInterestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Hakemler");

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HakemInterest",
                columns: table => new
                {
                    HakemsId = table.Column<int>(type: "int", nullable: false),
                    InterestsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakemInterest", x => new { x.HakemsId, x.InterestsId });
                    table.ForeignKey(
                        name: "FK_HakemInterest_Hakemler_HakemsId",
                        column: x => x.HakemsId,
                        principalTable: "Hakemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HakemInterest_Interests_InterestsId",
                        column: x => x.InterestsId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HakemInterest_InterestsId",
                table: "HakemInterest",
                column: "InterestsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HakemInterest");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Hakemler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

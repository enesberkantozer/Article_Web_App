using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateEntityName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Hakemler_HakemId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "HakemInterest");

            migrationBuilder.DropTable(
                name: "Hakemler");

            migrationBuilder.RenameColumn(
                name: "HakemId",
                table: "Ratings",
                newName: "ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_HakemId",
                table: "Ratings",
                newName: "IX_Ratings_ReviewerId");

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestReviewer",
                columns: table => new
                {
                    InterestsId = table.Column<int>(type: "int", nullable: false),
                    ReviewersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestReviewer", x => new { x.InterestsId, x.ReviewersId });
                    table.ForeignKey(
                        name: "FK_InterestReviewer_Interests_InterestsId",
                        column: x => x.InterestsId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestReviewer_Reviewers_ReviewersId",
                        column: x => x.ReviewersId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterestReviewer_ReviewersId",
                table: "InterestReviewer",
                column: "ReviewersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Reviewers_ReviewerId",
                table: "Ratings",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Reviewers_ReviewerId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "InterestReviewer");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "Ratings",
                newName: "HakemId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_ReviewerId",
                table: "Ratings",
                newName: "IX_Ratings_HakemId");

            migrationBuilder.CreateTable(
                name: "Hakemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hakemler", x => x.Id);
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

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Hakemler_HakemId",
                table: "Ratings",
                column: "HakemId",
                principalTable: "Hakemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

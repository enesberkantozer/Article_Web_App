using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class fixReviewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleReviewer",
                columns: table => new
                {
                    ArticlesId = table.Column<int>(type: "int", nullable: false),
                    ReviewersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleReviewer", x => new { x.ArticlesId, x.ReviewersId });
                    table.ForeignKey(
                        name: "FK_ArticleReviewer_Articles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleReviewer_Reviewers_ReviewersId",
                        column: x => x.ReviewersId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleReviewer_ReviewersId",
                table: "ArticleReviewer",
                column: "ReviewersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleReviewer");
        }
    }
}

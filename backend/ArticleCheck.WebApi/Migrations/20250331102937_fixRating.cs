using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class fixRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "RatingValue",
                table: "Ratings",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RatingValue",
                table: "Ratings",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}

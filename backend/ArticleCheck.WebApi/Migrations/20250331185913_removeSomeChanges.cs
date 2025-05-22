using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeSomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed_Article_Ids",
                table: "Reviewers");

            migrationBuilder.DropColumn(
                name: "Completed_Reviewer_Ids",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Completed_Article_Ids",
                table: "Reviewers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Completed_Reviewer_Ids",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

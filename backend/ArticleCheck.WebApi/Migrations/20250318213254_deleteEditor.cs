using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class deleteEditor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Editors_EditorId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Editors");

            migrationBuilder.DropIndex(
                name: "IX_Messages_EditorId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EditorId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EditorId",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditorId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EditorId",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Editors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EditorId",
                table: "Messages",
                column: "EditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Editors_EditorId",
                table: "Messages",
                column: "EditorId",
                principalTable: "Editors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

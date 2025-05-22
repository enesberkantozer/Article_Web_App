using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EditorId = table.Column<int>(type: "int", nullable: false),
                    UploadedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Editors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hakemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hakemler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    AuthorMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EditorId = table.Column<int>(type: "int", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Editors_EditorId",
                        column: x => x.EditorId,
                        principalTable: "Editors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    HakemId = table.Column<int>(type: "int", nullable: false),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Hakemler_HakemId",
                        column: x => x.HakemId,
                        principalTable: "Hakemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ArticleId",
                table: "Messages",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_EditorId",
                table: "Messages",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ArticleId",
                table: "Ratings",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_HakemId",
                table: "Ratings",
                column: "HakemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Editors");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Hakemler");
        }
    }
}

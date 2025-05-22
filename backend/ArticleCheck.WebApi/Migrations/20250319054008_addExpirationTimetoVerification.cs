using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addExpirationTimetoVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Verifications",
                newName: "ExpirationTime");

            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                table: "Verifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Verifications",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Verifications");

            migrationBuilder.RenameColumn(
                name: "ExpirationTime",
                table: "Verifications",
                newName: "Time");

            migrationBuilder.AlterColumn<string>(
                name: "EMail",
                table: "Verifications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications",
                column: "EMail");
        }
    }
}

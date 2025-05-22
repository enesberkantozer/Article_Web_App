using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleCheck.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications");

            migrationBuilder.DropColumn(
                name: "VerificationId",
                table: "Verifications");

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

            migrationBuilder.AddColumn<Guid>(
                name: "VerificationId",
                table: "Verifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verifications",
                table: "Verifications",
                column: "VerificationId");
        }
    }
}

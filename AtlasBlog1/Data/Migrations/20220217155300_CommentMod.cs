using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBlog1.Data.Migrations
{
    public partial class CommentMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModBody",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModDate",
                table: "Comment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModType",
                table: "Comment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModeratorId",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ModeratorId",
                table: "Comment",
                column: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ModeratorId",
                table: "Comment",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ModeratorId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ModeratorId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModBody",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModDate",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModType",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModeratorId",
                table: "Comment");
        }
    }
}

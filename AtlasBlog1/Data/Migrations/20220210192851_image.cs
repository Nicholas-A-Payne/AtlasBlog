using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBlog1.Data.Migrations
{
    public partial class image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Blogs",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "Blogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "Blogs");
        }
    }
}

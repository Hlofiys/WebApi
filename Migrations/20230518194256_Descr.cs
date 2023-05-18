using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Descr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Variants",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "UserInfo",
                table: "Users",
                type: "hstore",
                nullable: true,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "hstore");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Description",
                table: "Variants",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "UserInfo",
                table: "Users",
                type: "hstore",
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "hstore",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Description",
                table: "Items",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

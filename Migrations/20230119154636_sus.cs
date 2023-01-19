using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class sus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variant",
                table: "CartItems");

            migrationBuilder.AddColumn<int[]>(
                name: "Variants",
                table: "CartItems",
                type: "integer[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Variants",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "Variant",
                table: "CartItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.FCG.Infra.Migrations
{
    /// <inheritdoc />
    public partial class GameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Game",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "Game",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "Game");
        }
    }
}

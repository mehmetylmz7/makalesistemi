using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class hakemisimekle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Isim",
                table: "Hakemler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isim",
                table: "Hakemler");
        }
    }
}

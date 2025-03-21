using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class makaledurumeklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Makaleler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Makaleler");
        }
    }
}

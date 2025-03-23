using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class TakipNumarasi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TakipNumarasi",
                table: "Makaleler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakipNumarasi",
                table: "Makaleler");
        }
    }
}

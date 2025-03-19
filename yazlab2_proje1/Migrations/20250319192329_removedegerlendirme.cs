using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class removedegerlendirme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Hakemler_HakemId",
                table: "Degerlendirmeler");

            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirmeler_Makaleler_MakaleId",
                table: "Degerlendirmeler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Degerlendirmeler",
                table: "Degerlendirmeler");

            migrationBuilder.RenameTable(
                name: "Degerlendirmeler",
                newName: "Degerlendirme");

            migrationBuilder.RenameIndex(
                name: "IX_Degerlendirmeler_MakaleId",
                table: "Degerlendirme",
                newName: "IX_Degerlendirme_MakaleId");

            migrationBuilder.RenameIndex(
                name: "IX_Degerlendirmeler_HakemId",
                table: "Degerlendirme",
                newName: "IX_Degerlendirme_HakemId");

            migrationBuilder.AddColumn<string>(
                name: "HakemDegerlendirmesi",
                table: "Makaleler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DosyaYolu",
                table: "Anonimlestirmeler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Degerlendirme",
                table: "Degerlendirme",
                columns: new[] { "Id", "MakaleId", "HakemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirme_Hakemler_HakemId",
                table: "Degerlendirme",
                column: "HakemId",
                principalTable: "Hakemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirme_Makaleler_MakaleId",
                table: "Degerlendirme",
                column: "MakaleId",
                principalTable: "Makaleler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirme_Hakemler_HakemId",
                table: "Degerlendirme");

            migrationBuilder.DropForeignKey(
                name: "FK_Degerlendirme_Makaleler_MakaleId",
                table: "Degerlendirme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Degerlendirme",
                table: "Degerlendirme");

            migrationBuilder.DropColumn(
                name: "HakemDegerlendirmesi",
                table: "Makaleler");

            migrationBuilder.DropColumn(
                name: "DosyaYolu",
                table: "Anonimlestirmeler");

            migrationBuilder.RenameTable(
                name: "Degerlendirme",
                newName: "Degerlendirmeler");

            migrationBuilder.RenameIndex(
                name: "IX_Degerlendirme_MakaleId",
                table: "Degerlendirmeler",
                newName: "IX_Degerlendirmeler_MakaleId");

            migrationBuilder.RenameIndex(
                name: "IX_Degerlendirme_HakemId",
                table: "Degerlendirmeler",
                newName: "IX_Degerlendirmeler_HakemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Degerlendirmeler",
                table: "Degerlendirmeler",
                columns: new[] { "Id", "MakaleId", "HakemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Hakemler_HakemId",
                table: "Degerlendirmeler",
                column: "HakemId",
                principalTable: "Hakemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Degerlendirmeler_Makaleler_MakaleId",
                table: "Degerlendirmeler",
                column: "MakaleId",
                principalTable: "Makaleler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

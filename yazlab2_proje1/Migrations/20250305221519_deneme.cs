using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class deneme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Editorler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editorler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hakemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hakemler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yazarlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eposta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yazarlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Makaleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YazarId = table.Column<int>(type: "int", nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HakemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makaleler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Makaleler_Hakemler_HakemId",
                        column: x => x.HakemId,
                        principalTable: "Hakemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Makaleler_Yazarlar_YazarId",
                        column: x => x.YazarId,
                        principalTable: "Yazarlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Anonimlestirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakaleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anonimlestirmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anonimlestirmeler_Makaleler_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Makaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Degerlendirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    MakaleId = table.Column<int>(type: "int", nullable: false),
                    HakemId = table.Column<int>(type: "int", nullable: false),
                    HakemDegerlendirmesi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degerlendirmeler", x => new { x.Id, x.MakaleId, x.HakemId });
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_Hakemler_HakemId",
                        column: x => x.HakemId,
                        principalTable: "Hakemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_Makaleler_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Makaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MakaleId = table.Column<int>(type: "int", nullable: false),
                    IslemTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loglar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loglar_Makaleler_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Makaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anonimlestirmeler_MakaleId",
                table: "Anonimlestirmeler",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_HakemId",
                table: "Degerlendirmeler",
                column: "HakemId");

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_MakaleId",
                table: "Degerlendirmeler",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Loglar_MakaleId",
                table: "Loglar",
                column: "MakaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Makaleler_HakemId",
                table: "Makaleler",
                column: "HakemId");

            migrationBuilder.CreateIndex(
                name: "IX_Makaleler_YazarId",
                table: "Makaleler",
                column: "YazarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anonimlestirmeler");

            migrationBuilder.DropTable(
                name: "Degerlendirmeler");

            migrationBuilder.DropTable(
                name: "Editorler");

            migrationBuilder.DropTable(
                name: "Loglar");

            migrationBuilder.DropTable(
                name: "Makaleler");

            migrationBuilder.DropTable(
                name: "Hakemler");

            migrationBuilder.DropTable(
                name: "Yazarlar");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace makalesistemi.Migrations
{
    /// <inheritdoc />
    public partial class sohbettablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sohbet tablosunu oluştur
            migrationBuilder.CreateTable(
                name: "Sohbetler", // DbSet'te belirttiğiniz isim (Sohbetler)
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Otomatik artan birincil anahtar
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false), // Mesaj içeriği
                    GondericiId = table.Column<int>(type: "int", nullable: false), // Gönderen kullanıcı ID'si
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"), // Mesajın gönderildiği tarih (varsayılan değer: şu anki zaman)
                    MakaleId = table.Column<int>(type: "int", nullable: false) // Mesajın hangi makaleye ait olduğu
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sohbetler", x => x.Id); // Birincil anahtar tanımı

                    // GondericiId ile Yazar tablosu arasında foreign key ilişkisi
                    table.ForeignKey(
                        name: "FK_Sohbetler_Yazarlar_GondericiId",
                        column: x => x.GondericiId,
                        principalTable: "Yazarlar", // Yazar tablosunun adı
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade); // Yazar silinirse, ilişkili sohbetler de silinsin

                    // MakaleId ile Makale tablosu arasında foreign key ilişkisi
                    table.ForeignKey(
                        name: "FK_Sohbetler_Makaleler_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Makaleler", // Makale tablosunun adı
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade); // Makale silinirse, ilişkili sohbetler de silinsin
                });

            // Foreign key indekslerini oluştur
            migrationBuilder.CreateIndex(
                name: "IX_Sohbetler_GondericiId",
                table: "Sohbetler",
                column: "GondericiId");

            migrationBuilder.CreateIndex(
                name: "IX_Sohbetler_MakaleId",
                table: "Sohbetler",
                column: "MakaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sohbet tablosunu sil (rollback işlemi)
            migrationBuilder.DropTable(
                name: "Sohbetler");
        }
    }
}
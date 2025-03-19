using Aspose.Pdf;
using Aspose.Pdf.Text;
using System;
using System.IO;

namespace makalesistemi.Services
{
    public class PdfService
    {
        public string DegerlendirmeEkle(string mevcutPdfYolu, string hakemAdi, string degerlendirme)
        {
            try
            {
                // 🔹 Dosya yolunu Goruntule metodundaki gibi oluştur
                string tamDosyaYolu = Path.Combine("wwwroot", mevcutPdfYolu.TrimStart('/'));

                Console.WriteLine($"Tam Dosya Yolu: {tamDosyaYolu}");
                if (!File.Exists(tamDosyaYolu))
                {
                    throw new FileNotFoundException("PDF dosyası bulunamadı.", tamDosyaYolu);
                }

                // 🔹 Mevcut PDF dosyasını yükle
                Document pdfDocument = new Document(tamDosyaYolu);

                // 🔹 Yeni bir sayfa ekleyerek hakem değerlendirmesini ekle
                Page yeniSayfa = pdfDocument.Pages.Add();
                TextFragment baslik = new TextFragment($"📌 Hakem: {hakemAdi} - Değerlendirme");
                baslik.TextState.FontSize = 14;
                baslik.TextState.FontStyle = FontStyles.Bold;
                baslik.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.DarkBlue);
                yeniSayfa.Paragraphs.Add(baslik);

                TextFragment icerik = new TextFragment(degerlendirme);
                icerik.TextState.FontSize = 12;
                yeniSayfa.Paragraphs.Add(icerik);

                // 🔹 Güncellenmiş PDF dosyasını kaydet
                string yeniDosyaAdi = $"degerlendirilmis_{Path.GetFileName(mevcutPdfYolu)}";
                string yeniDosyaYolu = Path.Combine("wwwroot/pdfs", yeniDosyaAdi);

                pdfDocument.Save(yeniDosyaYolu);

                return $"/pdfs/{yeniDosyaAdi}"; // 🔹 Yeni dosya yolunu döndür
            }
            catch (Exception ex)
            {
                throw new Exception("PDF düzenlenirken bir hata oluştu.", ex);
            }
        }


    }
}

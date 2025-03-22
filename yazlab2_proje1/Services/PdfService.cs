using Aspose.Pdf;
using Aspose.Pdf.Text;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using makalesistemi.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.IO;
using iText.Layout.Element;
using ITextDocument = iText.Layout.Document; // 📌 iText Document sınıfını farklı adla tanımla
using ITextParagraph = iText.Layout.Element.Paragraph; // 📌 iText Paragraph için özel isim kullan

namespace makalesistemi.Services
{
    public class PdfService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PdfService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string DegerlendirmeEkle(string mevcutPdfYolu, string hakemAdi, string degerlendirme)
        {
            try
            {
                string tamDosyaYolu = Path.Combine("wwwroot", mevcutPdfYolu.TrimStart('/'));

                if (!File.Exists(tamDosyaYolu))
                {
                    throw new FileNotFoundException("PDF dosyası bulunamadı.", tamDosyaYolu);
                }

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(tamDosyaYolu);
                Page yeniSayfa = pdfDocument.Pages.Add();

                TextFragment baslik = new TextFragment($"📌 Hakem: {hakemAdi} - Değerlendirme")
                {
                    TextState = { FontSize = 14, FontStyle = FontStyles.Bold, ForegroundColor = Aspose.Pdf.Color.DarkBlue }
                };
                yeniSayfa.Paragraphs.Add(baslik);

                TextFragment icerik = new TextFragment(degerlendirme) { TextState = { FontSize = 12 } };
                yeniSayfa.Paragraphs.Add(icerik);

                string yeniDosyaAdi = $"degerlendirilmis_{Path.GetFileName(mevcutPdfYolu)}";
                string yeniDosyaYolu = Path.Combine("wwwroot/pdfs", yeniDosyaAdi);
                pdfDocument.Save(yeniDosyaYolu);

                return $"/pdfs/{yeniDosyaAdi}";
            }
            catch (Exception ex)
            {
                throw new Exception("PDF düzenlenirken bir hata oluştu.", ex);
            }
        }

        public string BirlesikPdfOlustur(string makaleDosyaYolu, string degerlendirme, string kayitDizini)
        {
            try
            {
                // 1️⃣ Mevcut makale PDF yolunu hazırla
                string tamMakaleYolu = Path.Combine("wwwroot", makaleDosyaYolu.TrimStart('/'));

                if (!File.Exists(tamMakaleYolu))
                {
                    throw new FileNotFoundException("Makale PDF dosyası bulunamadı.", tamMakaleYolu);
                }

                // 2️⃣ Hakem değerlendirmesini içeren geçici bir PDF oluştur
                string geciciDegerlendirmePdf = Path.Combine(kayitDizini, $"degerlendirme_{Guid.NewGuid()}.pdf");

                using (PdfWriter writer = new PdfWriter(geciciDegerlendirmePdf))
                using (PdfDocument pdfDocument = new PdfDocument(writer))
                using (ITextDocument document = new ITextDocument(pdfDocument)) // 📌 iText Document
                {
                    document.Add(new ITextParagraph("📌 Hakem Değerlendirmesi").SetFontSize(14));
                    document.Add(new ITextParagraph(degerlendirme).SetFontSize(12));
                }

                // 3️⃣ Birleştirilmiş PDF için yeni dosya adı oluştur
                string birlesikDosyaAdi = $"birlesik_{Path.GetFileName(makaleDosyaYolu)}";
                string birlesikDosyaYolu = Path.Combine(kayitDizini, birlesikDosyaAdi);

                // 4️⃣ PDF'leri birleştir
                using (PdfDocument makalePdf = new PdfDocument(new PdfReader(tamMakaleYolu)))
                using (PdfDocument degerlendirmePdf = new PdfDocument(new PdfReader(geciciDegerlendirmePdf)))
                using (PdfDocument birlesikPdf = new PdfDocument(new PdfWriter(birlesikDosyaYolu)))
                {
                    PdfMerger merger = new PdfMerger(birlesikPdf);
                    merger.Merge(makalePdf, 1, makalePdf.GetNumberOfPages());
                    merger.Merge(degerlendirmePdf, 1, degerlendirmePdf.GetNumberOfPages());
                }

                // 5️⃣ Geçici dosyayı temizle
                File.Delete(geciciDegerlendirmePdf);

                return $"/makaleler/{birlesikDosyaAdi}"; // 🌍 Yeni dosya yolunu döndür
            }
            catch (Exception ex)
            {
                throw new Exception("PDF işlemi sırasında hata oluştu.", ex);
            }
        }
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using makalesistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Canvas;


namespace makalesistemi
{
    public class PdfAnonymizationService
    {
        public string AnonymizePdf(string inputPdfPath, string outputPdfPath)
        {
            try
            {
                // 📌 Öncelikle, PDF'yi okuma modunda aç ve metni al
                string extractedText;
                using (PdfReader reader = new PdfReader(inputPdfPath))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    extractedText = "";
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        extractedText += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                    }
                } // 📌 Burada dosya kapanmış olacak!

                // 📌 Şimdi metni anonimleştir
                string anonymizedText = AnonymizeText(extractedText);

                // 📌 Yeni PDF dosyası oluştur ve anonimleştirilmiş metni kaydet
                using (PdfWriter writer = new PdfWriter(outputPdfPath))
                using (PdfDocument newPdfDoc = new PdfDocument(writer))
                {
                    Document document = new Document(newPdfDoc);
                    document.Add(new Paragraph(anonymizedText));
                }

                return outputPdfPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.ToString());
                throw;
            }
        }


        public string ExtractTextFromPdf(string filePath)
        {
            try
            {
                // 📌 Dosyanın var olup olmadığını kontrol et
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Hata: PDF dosyası bulunamadı -> {filePath}");
                    throw new FileNotFoundException("PDF dosyası bulunamadı.", filePath);
                }

                using PdfReader reader = new PdfReader(filePath);
                using PdfDocument pdfDoc = new PdfDocument(reader);
                string text = string.Empty;

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                }

                return text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.ToString());
                throw;
            }
        }

        private string AnonymizeText(string text)
        {
            string authorPattern = @"([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)\s*,\s*([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)";
            string institutionPattern = @"\b(University|Institute|Department|College|Lab|School)[^,.]+";
            string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            string orcidPattern = @"https://orcid\.org/\d{4}-\d{4}-\d{4}-\d{4}";

            text = Regex.Replace(text, authorPattern, "Yazar *****");
            text = Regex.Replace(text, institutionPattern, "Kurum *****");
            text = Regex.Replace(text, emailPattern, "Email *****");
            text = Regex.Replace(text, orcidPattern, "ORCID *****");

            return text;
        }

        public void SaveAnonymizedPdf(string anonymizedText, string outputPath)
        {
            try
            {
                // 📌 Çıkış klasörünü oluştur
                string directoryPath = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Dizin oluşturuluyor: {directoryPath}");
                    Directory.CreateDirectory(directoryPath);
                }

                // 📌 PDF kaydetme işlemi
                Console.WriteLine($"Anonymized PDF kaydediliyor -> {outputPath}");
                using FileStream fs = new FileStream(outputPath, FileMode.Create);
                using PdfWriter writer = new PdfWriter(fs);
                using PdfDocument pdfDoc = new PdfDocument(writer);
                Document document = new Document(pdfDoc);
                document.Add(new Paragraph(anonymizedText));
                document.Close();

                Console.WriteLine("PDF başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.ToString());
                throw;
            }
        }

    }
}
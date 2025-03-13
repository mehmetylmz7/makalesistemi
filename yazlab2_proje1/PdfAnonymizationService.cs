using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace makalesistemi
{
    public class PdfAnonymizationService
    {
        public void AnonymizePdf(string inputPdfPath, string outputPdfPath)
        {
            try
            {
                using (PdfReader reader = new PdfReader(inputPdfPath))
                using (PdfWriter writer = new PdfWriter(outputPdfPath))
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        PdfPage page = pdfDoc.GetPage(i);
                        PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

                        // Sayfadaki metni bul ve değiştir
                        ITextExtractionStrategy strategy = new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(page.GetPageSize()));
                        string text = PdfTextExtractor.GetTextFromPage(page, strategy);

                        // Metni anonimleştir
                        string anonymizedText = AnonymizeText(text);

                        // Orijinal metni sil
                        pdfCanvas.SaveState();
                        pdfCanvas.SetFillColor(ColorConstants.WHITE);
                        pdfCanvas.Rectangle(page.GetPageSize());
                        pdfCanvas.Fill();
                        pdfCanvas.RestoreState();

                        // Yeni metni ekle
                        Canvas canvas = new Canvas(pdfCanvas, page.GetPageSize());
                        canvas.Add(new Paragraph(anonymizedText));
                        canvas.Close();
                    }
                }

                Console.WriteLine("PDF başarıyla anonimleştirildi ve kaydedildi.");
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
    }
}
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

namespace makalesistemi
{
    public class PdfAnonymizationService
    {
        public string AnonymizePdf(string inputPdfPath, string outputPdfPath)
        {
            string extractedText = ExtractTextFromPdf(inputPdfPath);
            string anonymizedText = AnonymizeText(extractedText);
            SaveAnonymizedPdf(anonymizedText, outputPdfPath);
            return outputPdfPath;
        }

        private string ExtractTextFromPdf(string filePath)
        {
            using PdfReader reader = new PdfReader(filePath);
            using PdfDocument pdfDoc = new PdfDocument(reader);
            StringWriter text = new StringWriter();

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                text.WriteLine(pageText);
            }

            return text.ToString();
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

        private void SaveAnonymizedPdf(string anonymizedText, string outputPath)
        {
            using FileStream fs = new FileStream(outputPath, FileMode.Create);
            using PdfWriter writer = new PdfWriter(fs);
            using PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);

            document.Add(new Paragraph(anonymizedText));
            document.Close();
        }
    }
}
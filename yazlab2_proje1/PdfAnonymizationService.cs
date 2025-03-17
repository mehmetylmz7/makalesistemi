using Aspose.Pdf;
using Aspose.Pdf.Text;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class PdfAnonymizationService
{
    public void AnonymizePdf(string inputPath, string outputPath)
    {
        Document pdfDocument = new Document(inputPath);
        TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
        pdfDocument.Pages.Accept(textFragmentAbsorber);

        HashSet<string> processedKeys = new HashSet<string>();
        foreach (TextFragment textFragment in textFragmentAbsorber.TextFragments)
        {
            // Benzersiz anahtar oluştur
            string uniqueKey = $"{textFragment.Page.Number}_"
                + $"{Math.Round(textFragment.Rectangle.LLX, 4)}_"
                + $"{Math.Round(textFragment.Rectangle.LLY, 4)}_"
                + $"{Math.Round(textFragment.Rectangle.URX, 4)}_"
                + $"{Math.Round(textFragment.Rectangle.URY, 4)}_"
                + $"{DateTime.UtcNow.Ticks}";  // Zamanı anahtara ekleyerek benzersiz yapıyoruz

            if (processedKeys.Contains(uniqueKey))
            {
                Console.WriteLine($"Tekrarlanan anahtar: {uniqueKey}");
                continue;
            }

            string originalText = textFragment.Text;
            string anonymizedText = AnonymizeText(originalText);

            if (!string.IsNullOrWhiteSpace(anonymizedText) && originalText != anonymizedText)
            {
                try
                {
                    Console.WriteLine($"Metin değiştiriliyor: {originalText} -> {anonymizedText}");
                    textFragment.Text = anonymizedText;
                    textFragment.TextState.Font = FontRepository.FindFont("Arial");
                    textFragment.TextState.FontSize = textFragment.TextState.FontSize;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Hata! Key çakışması tespit edildi: {uniqueKey}");
                    throw;
                }
            }

            processedKeys.Add(uniqueKey);
        }

        pdfDocument.Save(outputPath);
    }

    private string AnonymizeText(string text)
    {
        try
        {
            string authorPattern = @"([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)\s*,\s*([A-Z][a-z]+(?:\s[A-Z][a-z]+)*)";
            string institutionPattern = @"\b(University|Institute|Department|College|Lab|School)\b";
            string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            string orcidPattern = @"https://orcid\.org/\d{4}-\d{4}-\d{4}-\d{4}";

            text = Regex.Replace(text, authorPattern, "Yazar *****", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, institutionPattern, "Kurum *****", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, emailPattern, "Email *****", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, orcidPattern, "ORCID *****", RegexOptions.IgnoreCase);

            return text;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

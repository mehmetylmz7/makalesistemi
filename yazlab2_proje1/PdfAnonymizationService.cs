using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.Text.RegularExpressions;

public class PdfAnonymizationService
{
    public void AnonymizePdf(string inputPath, string outputPath)
    {
        Document pdfDocument = new Document(inputPath);

        // PDF'teki tüm metni tarayacak bir absorber oluştur
        TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
        pdfDocument.Pages.Accept(textFragmentAbsorber);

        foreach (TextFragment textFragment in textFragmentAbsorber.TextFragments)
        {
            string originalText = textFragment.Text;
            string anonymizedText = AnonymizeText(originalText);

            if (originalText != anonymizedText) // Eğer değişiklik yapılıyorsa uygula
            {
                textFragment.Text = anonymizedText;
                textFragment.TextState.Font = FontRepository.FindFont("Arial"); // Font hatalarını önlemek için
                textFragment.TextState.FontSize = textFragment.TextState.FontSize; // Orijinal boyutu koru
               // textFragment.TextState.ApplyChanges(); // Değişiklikleri uygula
            }
        }

        pdfDocument.Save(outputPath);
    }

    private string AnonymizeText(string text)
    {
        // Genişletilmiş regex desenleri
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
}

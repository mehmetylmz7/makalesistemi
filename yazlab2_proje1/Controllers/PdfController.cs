using System.IO;
using makalesistemi;
using Microsoft.AspNetCore.Mvc;

[Route("api/pdf")]
[ApiController]
public class PdfController : ControllerBase
{
    private readonly PdfAnonymizationService _pdfService;

    public PdfController()
    {
        _pdfService = new PdfAnonymizationService();
    }

    [HttpPost("anonymize")]
    public IActionResult AnonymizePdf([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Lütfen bir PDF dosyası yükleyin.");
        }

        try
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "guncellenenmakaleler");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = "anonimleştirilmiş_" + Path.GetFileNameWithoutExtension(file.FileName) + ".pdf";
            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "makaleler", file.FileName);
            string outputFilePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(inputFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string anonymizedFilePath = _pdfService.AnonymizePdf(inputFilePath, outputFilePath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(anonymizedFilePath);
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Bir hata oluştu: " + ex.Message);
        }
    }
}

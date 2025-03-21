using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using makalesistemi.Models;
using makalesistemi.Services;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace makalesistemi.Controllers
{
    public class HakemController : Controller
    {
        private readonly Context _context;
        private readonly PdfService _pdfService;
        private readonly AesEncryptionService _encryptionService;

        public HakemController(Context context, PdfService pdfService, AesEncryptionService encryptionService)
        {
            _context = context;
            _pdfService = pdfService;
            _encryptionService = encryptionService;
        }

        [HttpPost]
        public IActionResult Degerlendir(int makaleId, string degerlendirme)
        {
            int? hakemId = HttpContext.Session.GetInt32("HakemID");
            if (hakemId == null)
            {
                return RedirectToAction("Login", "Hakem");
            }

            var makale = _context.Makaleler.Find(makaleId);
            if (makale == null || makale.HakemId != hakemId)
            {
                return NotFound();
            }

            makale.HakemDegerlendirmesi = _encryptionService.Encrypt(degerlendirme);
            _context.Makaleler.Update(makale);
            _context.SaveChanges();

            return RedirectToAction("Panel", "Hakem");
        }

        [HttpPost]
        public IActionResult DegerlendirmeSil(int makaleId)
        {
            var makale = _context.Makaleler.Find(makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            makale.HakemDegerlendirmesi = null;
            _context.SaveChanges();

            return RedirectToAction("Panel", "Hakem");
        }

        [HttpPost]
        public IActionResult DegerlendirmeGuncelle(int makaleId, string guncelDegerlendirme)
        {
            var makale = _context.Makaleler.Find(makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            makale.HakemDegerlendirmesi = _encryptionService.Encrypt(guncelDegerlendirme);
            _context.SaveChanges();

            return RedirectToAction("Panel", "Hakem");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var hakemler = await _context.Hakemler.ToListAsync();
            return View(hakemler);
        }

        [HttpPost]
        public IActionResult Giris(int hakemId)
        {
            HttpContext.Session.SetInt32("HakemID", hakemId);
            return RedirectToAction("Panel");
        }

        public async Task<IActionResult> Panel()
        {
            int? hakemId = HttpContext.Session.GetInt32("HakemID");
            if (hakemId == null)
            {
                return RedirectToAction("Login");
            }

            var makaleler = await _context.Makaleler
                .Where(m => m.HakemId == hakemId && m.Durum == ArticleStatus.HakemeIletildi)
                .ToListAsync();

            foreach (var makale in makaleler)
            {
                if (!string.IsNullOrEmpty(makale.HakemDegerlendirmesi))
                {
                    makale.HakemDegerlendirmesi = _encryptionService.Decrypt(makale.HakemDegerlendirmesi);
                }
            }

            return View(makaleler);
        }

        [HttpPost]
        public IActionResult EditoreIlet(int makaleId)
        {
            var makale = _context.Makaleler.Find(makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            // Dosyayı okuyup yeni sayfa eklemek için Aspose.PDF kullanıyoruz
            string originalFilePath = Path.Combine("wwwroot", makale.DosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(originalFilePath))
            {
                return NotFound("Makale dosyası mevcut değil.");
            }

            // PDF dosyasını açıyoruz
            var document = new Aspose.Pdf.Document(originalFilePath);

            // Yeni sayfa ekliyoruz
            var newPage = document.Pages.Add();

            // Hakem değerlendirmesini ekliyoruz
            string hakemDegerlendirmesi = _encryptionService.Decrypt(makale.HakemDegerlendirmesi);  // Decrypt edilmiş metin
            var textFragment = new Aspose.Pdf.Text.TextFragment(hakemDegerlendirmesi);
            textFragment.Position = new Aspose.Pdf.Text.Position(100, 700); // Konumu belirleyin
            newPage.Paragraphs.Add(textFragment);

            // Yeni dosya yolu
            string newFileName = $"{Guid.NewGuid()}.pdf";  // Yeni dosya ismi
            string newFilePath = Path.Combine("wwwroot", "makaleler", newFileName);

            // Yeni PDF dosyasını kaydediyoruz
            document.Save(newFilePath);

            // Makale nesnesindeki yeni dosya yolunu güncelliyoruz
            makale.DegerlendirmeDosyaYolu = $"/makaleler/{newFileName}";
            makale.Durum = ArticleStatus.EditoreIletildi; // Durum güncelleniyor
            _context.SaveChanges();

            return RedirectToAction("Panel", "Hakem");
        }


        public async Task<IActionResult> Goruntule(int id)
        {
            var makale = await _context.Makaleler.FindAsync(id);
            if (makale == null || string.IsNullOrEmpty(makale.AnonimDosyaYolu))
            {
                return NotFound("Makale bulunamadı veya dosya yolu mevcut değil.");
            }

            string filePath = Path.Combine("wwwroot", makale.AnonimDosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Makale dosyası mevcut değil.");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", "makale.pdf");
        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Remove("HakemID");
            return RedirectToAction("Login");
        }
    }
}

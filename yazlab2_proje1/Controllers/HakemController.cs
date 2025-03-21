using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using makalesistemi.Models;
using makalesistemi.Services;
using System.IO;

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
                .Where(m => m.HakemId == hakemId)
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

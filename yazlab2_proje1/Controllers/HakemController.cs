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

        public HakemController(Context context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
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

            makale.HakemDegerlendirmesi = degerlendirme;
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

            return View(makaleler);
        }

        public async Task<IActionResult> Goruntule(int id)
        {
            var makale = await _context.Makaleler.FindAsync(id);
            if (makale == null || string.IsNullOrEmpty(makale.DosyaYolu))
            {
                return NotFound("Makale bulunamadı veya dosya yolu mevcut değil.");
            }

            string filePath = Path.Combine("wwwroot", makale.DosyaYolu.TrimStart('/'));
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
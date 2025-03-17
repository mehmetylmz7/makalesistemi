using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using makalesistemi.Models;

namespace makalesistemi.Controllers
{
    public class HakemController : Controller
    {
        private readonly Context _context;

        public HakemController(Context context)
        {
            _context = context;
        }

        // 1️⃣ Hakem Giriş Ekranı (Hakemlerin Listesi)
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var hakemler = await _context.Hakemler.ToListAsync();
            return View(hakemler);
        }

        // 2️⃣ Seçilen Hakemin Giriş Yapması
        [HttpPost]
        public IActionResult Giris(int hakemId)
        {
            HttpContext.Session.SetInt32("HakemID", hakemId);
            return RedirectToAction("Panel");
        }

        // 3️⃣ Hakem Paneli - Kendisine Atanan Makaleleri Görüntüleme
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

        // 4️⃣ Makale Görüntüleme
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

        // 5️⃣ Hakemin Oturumu Kapatması
        public IActionResult Cikis()
        {
            HttpContext.Session.Remove("HakemID");
            return RedirectToAction("Login");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using makalesistemi.Models;
using makalesistemi.Services;

namespace makalesistemi.Controllers
{
    public class HakemController : Controller
    {
        private readonly Context _context;
        private readonly PdfService _pdfService; // 🔹 PdfService ekle

        public HakemController(Context context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService; // 🔹 Bağımlılığı Constructor'dan al
        }

        [HttpPost]
        public IActionResult Degerlendir(int makaleId, string degerlendirme)
        {
            // HakemId oturumdan alınır
            int? hakemId = HttpContext.Session.GetInt32("HakemID");
            if (hakemId == null)
            {
                return RedirectToAction("Login", "Hakem");
            }

            // Değerlendirme var mı diye kontrol et
            var mevcutDegerlendirme = _context.Degerlendirmeler
                .FirstOrDefault(d => d.MakaleId == makaleId && d.HakemId == hakemId);

            if (mevcutDegerlendirme != null)
            {
                // Değerlendirme varsa, güncelle
                mevcutDegerlendirme.HakemDegerlendirmesi = degerlendirme;
                _context.Degerlendirmeler.Update(mevcutDegerlendirme);
            }
            else
            {
                // Değerlendirme yoksa, yeni ekle
                var yeniDegerlendirme = new Degerlendirme
                {
                    MakaleId = makaleId,
                    HakemId = hakemId.Value,
                    HakemDegerlendirmesi = degerlendirme
                };
                _context.Degerlendirmeler.Add(yeniDegerlendirme);
            }

            // Veritabanında değişiklikleri kaydet
            _context.SaveChanges();

            // Hakem paneline yönlendir
            return RedirectToAction("Panel", "Hakem");
        }

        [HttpPost]
        public IActionResult DegerlendirmeGuncelle(int degerlendirmeId, string guncelDegerlendirme)
        {
            // Veritabanında ilgili değerlendirmeyi bul
            var degerlendirme = _context.Degerlendirmeler.Find(degerlendirmeId);
            if (degerlendirme == null)
            {
                return NotFound(); // Değerlendirme bulunamazsa hata döner
            }

            // Değerlendirmeyi güncelle
            degerlendirme.HakemDegerlendirmesi = guncelDegerlendirme;
            _context.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("Panel", "Hakem"); // Hakem paneline yönlendir
        }

        [HttpPost]
        public IActionResult DegerlendirmeSil(int degerlendirmeId)
        {
            // Silinecek değerlendirmeyi veritabanında bul
            var degerlendirme = _context.Degerlendirmeler.Find(degerlendirmeId);
            if (degerlendirme == null)
            {
                return NotFound(); // Değerlendirme bulunamazsa hata döner
            }

            // Değerlendirmeyi sil
            _context.Degerlendirmeler.Remove(degerlendirme);
            _context.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("Panel", "Hakem"); // Hakem paneline yönlendir
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

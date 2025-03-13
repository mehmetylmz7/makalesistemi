using makalesistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace makalesistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly PdfAnonymizationService _pdfAnonymizationService;

        public AdminController(Context context, IWebHostEnvironment hostEnvironment, PdfAnonymizationService pdfAnonymizationService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _pdfAnonymizationService = pdfAnonymizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Panel()
        {
            var anonimMakaleListesi = await _context.Makaleler
                .Where(m => _context.Anonimlestirmeler.Any(a => a.MakaleId == m.Id))
                .ToListAsync();

            var anonimDegilMakaleListesi = await _context.Makaleler
                .Where(m => !_context.Anonimlestirmeler.Any(a => a.MakaleId == m.Id))
                .ToListAsync();

            var hakemListesi = await _context.Hakemler.ToListAsync();

            return View(Tuple.Create(anonimMakaleListesi, anonimDegilMakaleListesi, hakemListesi));
        }

        [HttpGet]
        public async Task<IActionResult> Goruntule(int id)
        {
            var makale = await _context.Makaleler.FindAsync(id);
            if (makale == null || string.IsNullOrEmpty(makale.DosyaYolu) || !System.IO.File.Exists($"wwwroot{makale.DosyaYolu}"))
            {
                return NotFound("Makale bulunamadı veya dosya mevcut değil.");
            }

            return File(System.IO.File.ReadAllBytes($"wwwroot{makale.DosyaYolu}"), "application/pdf", "makale.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Anonimlestir(int id)
        {
            var makale = await _context.Makaleler.FindAsync(id);
            if (makale == null)
            {
                return NotFound("Makale bulunamadı.");
            }

            var anonimKayit = await _context.Anonimlestirmeler.FirstOrDefaultAsync(a => a.MakaleId == id);
            if (anonimKayit != null)
            {
                ViewData["Message"] = "Bu makale zaten anonimleştirildi!";
                return RedirectToAction("Panel");
            }

            // 📌 Dosya yollarını oluştur
            string inputPath = Path.Combine(_hostEnvironment.WebRootPath, makale.DosyaYolu.TrimStart('/'));
            string outputDir = Path.Combine(_hostEnvironment.WebRootPath, "makaleler");

            // 📌 Hata ayıklama için yolları yazdır
            Console.WriteLine($"Giriş Dosya Yolu (inputPath): {inputPath}");
            Console.WriteLine($"Çıkış Klasörü (outputDir): {outputDir}");

            if (!System.IO.File.Exists(inputPath))
            {
                Console.WriteLine("Hata: Giriş PDF dosyası bulunamadı!");
                return NotFound("Makale dosyası mevcut değil.");
            }

            // 📌 Çıkış dizini yoksa oluştur
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine("Çıkış dizini oluşturuldu.");
            }

            string outputPath = Path.Combine(outputDir, Path.GetFileName(makale.DosyaYolu));

            // 📌 Çıkış yolunu yazdır
            Console.WriteLine($"Çıkış Dosya Yolu (outputPath): {outputPath}");

            _pdfAnonymizationService.AnonymizePdf(inputPath, outputPath);

            var yeniAnonimlestirme = new Anonimlestirme { MakaleId = id };
            _context.Anonimlestirmeler.Add(yeniAnonimlestirme);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla anonimleştirildi!";
            return RedirectToAction("Panel");
        }

        // 📌 Hakeme Yönlendirme
        [HttpPost]
        public async Task<IActionResult> HakemeYolla(int makaleId, int hakemId)
        {
            var makale = await _context.Makaleler.FindAsync(makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            bool anonimMi = _context.Anonimlestirmeler.Any(a => a.MakaleId == makaleId);
            if (!anonimMi)
            {
                ViewData["Message"] = "Bu makale henüz anonimleştirilmedi!";
                return RedirectToAction("Panel");
            }

            makale.HakemId = hakemId;
            _context.Makaleler.Update(makale);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla hakeme yönlendirildi!";
            return RedirectToAction("Panel");
        }
    }
}

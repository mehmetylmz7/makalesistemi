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
            if (makale == null || string.IsNullOrEmpty(makale.DosyaYolu))
            {
                return NotFound("Makale bulunamadı veya dosya yolu mevcut değil.");
            }

            string filePath = Path.Combine(_hostEnvironment.WebRootPath, makale.DosyaYolu.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Makale dosyası mevcut değil.");
            }

            // 📌 Dosyanın kilitli olup olmadığını kontrol et
            if (IsFileLocked(filePath))
            {
                return BadRequest("Makale dosyası şu anda başka bir işlem tarafından kullanılıyor. Lütfen tekrar deneyin.");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", "makale.pdf");
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
            string outputPath = Path.Combine(outputDir, Path.GetFileName(makale.DosyaYolu));

            Console.WriteLine($"Giriş Dosya Yolu (inputPath): {inputPath}");
            Console.WriteLine($"Çıkış Dosya Yolu (outputPath): {outputPath}");

            if (!System.IO.File.Exists(inputPath))
            {
                Console.WriteLine("Hata: Giriş PDF dosyası bulunamadı!");
                return NotFound("Makale dosyası mevcut değil.");
            }

            // 📌 Dosyanın kilitli olup olmadığını kontrol et
            if (IsFileLocked(inputPath))
            {
                Console.WriteLine("Hata: Dosya başka bir işlem tarafından kullanılıyor.");
                return BadRequest("Makale dosyası şu anda başka bir işlem tarafından kullanılıyor. Lütfen tekrar deneyin.");
            }

            // 📌 Çıkış dizini yoksa oluştur
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine("Çıkış dizini oluşturuldu.");
            }

            // 📌 Dosya paylaşımını düzenle ve işlemi gerçekleştir
            using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                _pdfAnonymizationService.AnonymizePdf(inputPath, outputPath);
            }

            var yeniAnonimlestirme = new Anonimlestirme { MakaleId = id };
            _context.Anonimlestirmeler.Add(yeniAnonimlestirme);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla anonimleştirildi!";
            return RedirectToAction("Panel");
        }

        // 📌 Dosyanın kullanılabilir olup olmadığını kontrol eden metot
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false; // Dosya kullanılabilir
                }
            }
            catch (IOException)
            {
                return true; // Dosya kilitlenmiş
            }
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

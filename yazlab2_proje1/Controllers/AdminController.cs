using makalesistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

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

        [HttpPost]
        public async Task<IActionResult> MakaleKonusunuBul(int makaleId)
        {
            try
            {
                var makale = await _context.Makaleler.FindAsync(makaleId);
                if (makale == null)
                {
                    return Json(new { success = false, message = "Makale bulunamadı." });
                }

                string dosyaYolu = Path.Combine(_hostEnvironment.WebRootPath, makale.AnonimDosyaYolu.TrimStart('/'));
                if (!System.IO.File.Exists(dosyaYolu))
                {
                    return Json(new { success = false, message = "Makale dosyası mevcut değil." });
                }

                string jsonDosyaYolu = Path.Combine(_hostEnvironment.WebRootPath, "json", "konular.json");
                if (!System.IO.File.Exists(jsonDosyaYolu))
                {
                    return Json(new { success = false, message = "Konular JSON dosyası bulunamadı." });
                }

                string makaleIcerigi = await System.IO.File.ReadAllTextAsync(dosyaYolu);
                string jsonVeri = await System.IO.File.ReadAllTextAsync(jsonDosyaYolu);
                var konuVerileri = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonVeri);

                var konuSayac = new Dictionary<string, int>();
                foreach (var konu in konuVerileri)
                {
                    int sayac = konu.Value.Count(kelime => makaleIcerigi.Contains(kelime, StringComparison.OrdinalIgnoreCase));
                    konuSayac[konu.Key] = sayac;
                }

                var enIyiKonu = konuSayac.OrderByDescending(k => k.Value).FirstOrDefault();
                string mesaj = enIyiKonu.Value > 0
                    ? $"<p>Makale büyük ihtimalle '<strong>{enIyiKonu.Key}</strong>' konusundadır.</p>" +
                      $"<p>Eşleşen anahtar kelime sayısı: {enIyiKonu.Value}</p>"
                    : "<p>Makalenin konusu belirlenemedi.</p>";

                return Json(new { success = true, message = mesaj });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Panel()
        {
            var anonimMakaleListesi = await _context.Makaleler
                .Where(m => !string.IsNullOrEmpty(m.AnonimDosyaYolu)) // Anonim dosya yolu dolu olanlar
                .ToListAsync();

            var anonimDegilMakaleListesi = await _context.Makaleler
                .Where(m => string.IsNullOrEmpty(m.AnonimDosyaYolu)) // Anonim dosya yolu boş olanlar
                .ToListAsync();

           var editoreIletilenMakaleListesi = await _context.Makaleler
                .Where(m => m.Durum == ArticleStatus.EditoreIletildi)
                 .ToListAsync();

            var hakemListesi = await _context.Hakemler.ToListAsync();

            return View(Tuple.Create(anonimMakaleListesi, anonimDegilMakaleListesi,editoreIletilenMakaleListesi, hakemListesi));
        }
        public IActionResult Sohbet(int? makaleId)
        {
            // Admin'in görebileceği tüm makaleleri getir (DropDown için)
            var makaleler = _context.Makaleler.Include(m => m.Yazar).ToList();

            // Seçili makaleye ait mesajları getir
            var mesajlar = makaleId.HasValue
                ? _context.Sohbetler
                    .Include(s => s.Gonderici)
                    .Where(s => s.MakaleId == makaleId)
                    .OrderBy(s => s.Tarih)
                    .ToList()
                : new List<Sohbet>();

            ViewBag.Makaleler = makaleler;
            ViewBag.SeciliMakaleId = makaleId;

            return View(mesajlar);
        }

        [HttpPost]
        public IActionResult MesajGonder(int makaleId, string mesajIcerik)
        {
            if (string.IsNullOrWhiteSpace(mesajIcerik))
            {
                TempData["Hata"] = "Mesaj boş olamaz!";
                return RedirectToAction("Sohbet", new { makaleId });
            }

            // Admin ID'si (Admin giriş sistemine göre dinamik yapılabilir)
            int adminId = 2; // Admin ID sistemde nasıl tutuluyorsa ona göre değiştirilebilir.

            var yeniMesaj = new Sohbet
            {
                GondericiId = adminId,
                Icerik = mesajIcerik,
                MakaleId = makaleId,
                Tarih = DateTime.Now
            };

            _context.Sohbetler.Add(yeniMesaj);
            _context.SaveChanges();

            return RedirectToAction("Sohbet", new { makaleId });
        }

        [HttpPost]
        public async Task<IActionResult> YazaraIlet(int makaleId)
        {
            var makale = await _context.Makaleler.FindAsync(makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            // Makalenin durumunu "Yazara İletildi" olarak güncelle
            makale.Durum = ArticleStatus.YazaraIletildi;
            _context.Makaleler.Update(makale);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla yazara iletildi!";
            return RedirectToAction("Panel");
        }




        [HttpGet]
        public async Task<IActionResult> Goruntule(int id)
        {
            var makale = await _context.Makaleler.FindAsync(id);
            if (makale == null)
            {
                return NotFound("Makale bulunamadı.");
            }

            // Öncelikli olarak AnonimDosyaYolu'nu kullan, yoksa DosyaYolu'nu kullan
            string dosyaYolu = !string.IsNullOrEmpty(makale.AnonimDosyaYolu)
                ? makale.AnonimDosyaYolu
                : makale.DosyaYolu;

            if (string.IsNullOrEmpty(dosyaYolu))
            {
                return NotFound("Makale için geçerli bir dosya yolu bulunamadı.");
            }

            string filePath = Path.Combine(_hostEnvironment.WebRootPath, dosyaYolu.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Makale dosyası mevcut değil.");
            }

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

            if (!string.IsNullOrEmpty(makale.AnonimDosyaYolu))
            {
                ViewData["Message"] = "Bu makale zaten anonimleştirildi!";
                return RedirectToAction("Panel");
            }

            string inputPath = Path.Combine(_hostEnvironment.WebRootPath, makale.DosyaYolu.TrimStart('/'));
            string outputDir = Path.Combine(_hostEnvironment.WebRootPath, "makaleler");
            string outputPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(makale.DosyaYolu) + "_anonim.pdf");

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (!System.IO.File.Exists(inputPath))
            {
                return NotFound("Makale dosyası mevcut değil.");
            }

            try
            {
                _pdfAnonymizationService.AnonymizePdf(inputPath, outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Anonimleştirme hatası: {ex.Message}");
                throw;
            }

            // Anonimleştirme kaydı ekleme
           // var yeniAnonimlestirme = new Anonimlestirme { MakaleId = id };
           // _context.Anonimlestirmeler.Add(yeniAnonimlestirme);

            // 🔹 Makale tablosunda AnonimDosyaYolu'nu güncelle
            makale.AnonimDosyaYolu = "/makaleler/" + Path.GetFileName(outputPath);
            makale.Durum = ArticleStatus.Anonimlesti;
            _context.Makaleler.Update(makale);

            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla anonimleştirildi!";
            return RedirectToAction("Panel");
        }


        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        [HttpPost]
        public async Task<IActionResult> HakemeYolla(int makaleId, int hakemId)
        {
            var makale = await _context.Makaleler.FindAsync(makaleId);
            if (makale == null)
            {
                return NotFound();
            }
            /* 
            if (!_context.Anonimlestirmeler.Any(a => a.MakaleId == makaleId))
            {
                ViewData["Message"] = "Bu makale henüz anonimleştirilmedi!";
                return RedirectToAction("Panel");
            } */

            makale.HakemId = hakemId;
            makale.Durum=ArticleStatus.HakemeIletildi;
            _context.Makaleler.Update(makale);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla hakeme yönlendirildi!";
            return RedirectToAction("Panel");
        }
    }
}

using makalesistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace makalesistemi.Controllers
{
    public class MakaleController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MakaleController(Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Yukle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Yukle(string Eposta, IFormFile Dosya)
        {
            if (string.IsNullOrEmpty(Eposta) || Dosya == null || Dosya.Length == 0)
            {
                ViewData["Message"] = "Lütfen geçerli bir e-posta adresi girin ve bir PDF dosyası seçin.";
                return View();
            }

            var mevcutYazar = await _context.Yazarlar.FirstOrDefaultAsync(y => y.Eposta == Eposta);
            if (mevcutYazar == null)
            {
                mevcutYazar = new Yazar { Eposta = Eposta };
                _context.Yazarlar.Add(mevcutYazar);
                await _context.SaveChangesAsync();
            }

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(Dosya.FileName);
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "makaleler");
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Dosya.CopyToAsync(fileStream);
            }

            // 📌 Rastgele 3 harf ve 3 rakamlı takip numarası oluşturma
            string takipNumarasi;
            do
            {
                takipNumarasi = GenerateRandomString(3) + new Random().Next(100, 999).ToString(); // Örn: ABC123
            } while (await _context.Makaleler.AnyAsync(m => m.TakipNumarasi == takipNumarasi));

            var yeniMakale = new Makale
            {
                YazarId = mevcutYazar.Id,
                DosyaYolu = "/makaleler/" + uniqueFileName,
                HakemId = null,
                TakipNumarasi = takipNumarasi // 🔹 Takip numarasını ata
            };
            _context.Makaleler.Add(yeniMakale);
            await _context.SaveChangesAsync();

            ViewData["Message"] = $"Makale başarıyla yüklendi! Takip Numaranız: {takipNumarasi}";
            return View();
        }

        // 📌 Rastgele 3 harfli string oluşturma fonksiyonu
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            return new string(Enumerable.Range(0, length)
                                         .Select(_ => chars[random.Next(chars.Length)])
                                         .ToArray());
        }

        // Makale Durum Sayfası (GET)
        [HttpGet]
        public IActionResult Durum()
        {
            return View();
        }

        // Takip Numarası ile Makale Durumu (GET)
        // POST: Makale/Durum
        [HttpPost]
        public async Task<IActionResult> Durum(string takipNumarasi)
        {
            if (string.IsNullOrEmpty(takipNumarasi))
            {
                ViewData["ErrorMessage"] = "Lütfen bir takip numarası girin.";
                return View();
            }

            var makale = await _context.Makaleler
                .Include(m => m.Yazar)
                .Include(m => m.Hakem)
                .FirstOrDefaultAsync(m => m.TakipNumarasi == takipNumarasi);

            if (makale == null)
            {
                ViewData["ErrorMessage"] = "Belirtilen takip numarasına ait makale bulunamadı.";
                return View();
            }

            return View("DurumDetay", makale);
        }



    }
}
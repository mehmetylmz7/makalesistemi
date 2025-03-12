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

            var yeniMakale = new Makale
            {
                YazarId = mevcutYazar.Id,
                DosyaYolu = "/makaleler/" + uniqueFileName,
                HakemId = null
            };
            _context.Makaleler.Add(yeniMakale);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Makale başarıyla yüklendi!";
            return View();
        }
    }
}
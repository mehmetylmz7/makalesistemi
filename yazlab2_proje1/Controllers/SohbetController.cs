using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using makalesistemi.Models;

namespace makalesistemi.Controllers
{
   

    public class SohbetController : Controller
    {
        private readonly Context _context;

        public SohbetController(Context context)
        {
            _context = context;
        }

        // Makaleye ait mesajları getir
        public IActionResult Index(int makaleId)
        {
            var mesajlar = _context.Sohbetler
                .Where(m => m.MakaleId == makaleId)
                .OrderBy(m => m.Tarih)
                .ToList();

            ViewBag.KullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            return View(mesajlar);
        }

        [HttpPost]
        public IActionResult MesajGonder(int makaleId, string icerik)
        {
            var makale = _context.Makaleler.Include(m => m.Sohbetler).FirstOrDefault(m => m.Id == makaleId);
            if (makale == null)
            {
                return NotFound();
            }

            int gondericiId = makale.YazarId; // Gönderici ID, makale yazarına eşitleniyor.

            var mesaj = new Sohbet
            {
                Icerik = icerik,
                GondericiId = gondericiId,
                MakaleId = makaleId,
                Tarih = DateTime.Now
            };

            _context.Sohbetler.Add(mesaj);
            _context.SaveChanges();

            // Güncellenmiş mesaj listesini döndür
            var mesajlar = makale.Sohbetler.OrderBy(m => m.Tarih).ToList();
            return PartialView("_SohbetMesajlari", mesajlar);
        }

        [HttpGet]
        public async Task<IActionResult> MesajlariYenile(int makaleId)
        {
            var mesajlar = await _context.Sohbetler
                .Where(m => m.MakaleId == makaleId)
                .OrderBy(m => m.Tarih)
                .ToListAsync();

            return PartialView("_SohbetMesajlari", mesajlar);
        }



    }

}

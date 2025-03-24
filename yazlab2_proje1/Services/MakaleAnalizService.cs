using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace makalesistemi.Services

{
    public class MakaleAnalizService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public MakaleAnalizService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string BelirleMakaleKonusu(string makaleIcerik)
        {
            string jsonDosyaYolu = Path.Combine(_hostEnvironment.WebRootPath, "konular.json");

            if (!File.Exists(jsonDosyaYolu))
                return "Bilinmeyen Konu";

            string jsonIcerik = File.ReadAllText(jsonDosyaYolu);
            var konular = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonIcerik);

            Dictionary<string, int> eslesmeSayilari = new Dictionary<string, int>();

            foreach (var konu in konular)
            {
                int eslesme = konu.Value.Count(kelime => makaleIcerik.Contains(kelime, StringComparison.OrdinalIgnoreCase));
                eslesmeSayilari[konu.Key] = eslesme;
            }

            return eslesmeSayilari.OrderByDescending(x => x.Value).FirstOrDefault().Key ?? "Bilinmeyen Konu";
        }
    }

}

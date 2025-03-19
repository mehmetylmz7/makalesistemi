using makalesistemi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace makalesistemi.Models
{
    public enum ArticleStatus
    {
        Beklemede,
        İnceleniyor,
        RevizeEdildi,
        Onaylandı,
        Reddedildi
    }

    public class Makale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int YazarId { get; set; }

        [Required]
        public string DosyaYolu { get; set; }

        public int? HakemId { get; set; }  // Atanan hakem

        public string HakemDegerlendirmesi { get; set; }  // 🔹 Hakem değerlendirmesi buraya eklendi

        [ForeignKey("YazarId")]
        public Yazar Yazar { get; set; }

        [ForeignKey("HakemId")]
        public Hakem Hakem { get; set; }

        public List<Log> Loglar { get; set; } = new();
        public List<Anonimlestirme> Anonimlestirmeler { get; set; } = new();
    }
}

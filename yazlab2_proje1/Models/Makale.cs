using makalesistemi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace makalesistemi.Models
{
    public enum ArticleStatus
    {
        Yüklendi,          // Makale sisteme yüklendi
        Anonimlesti,       // Makale anonimleştirildi
        HakemeIletildi,    // Hakeme gönderildi
        EditoreIletildi,   // Editöre gönderildi
        YazaraIletildi     // Yazara geri gönderildi
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

        public string? HakemDegerlendirmesi { get; set; }  // 🔹 Hakem değerlendirmesi buraya eklendi

        public string? AnonimDosyaYolu { get; set; }  // 🔹 Anonimleştirilmiş dosyanın yolu

        public string? DegerlendirmeDosyaYolu { get; set; }  // 🔹 Değerlendirme dosyasının yolu

        public string? TakipNumarasi { get; set; }  // 🔹 Makaleye verilen takip numarası


        [ForeignKey("YazarId")]
        public Yazar Yazar { get; set; }

        [ForeignKey("HakemId")]
        public Hakem Hakem { get; set; }

        public ArticleStatus Durum { get; set; } = ArticleStatus.Yüklendi;

        public List<Log> Loglar { get; set; } = new();
        public List<Anonimlestirme> Anonimlestirmeler { get; set; } = new();
        public List<Sohbet> Sohbetler { get; set; } = new();
    }
}

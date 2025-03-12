using makalesistemi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace makalesistemi.Models
{
    public class Hakem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Isim { get; set; } // Hakemin ismi
        public List<Makale> Makaleler { get; set; } = new();
        public List<Degerlendirme> Degerlendirmeler { get; set; } = new();
    }
}


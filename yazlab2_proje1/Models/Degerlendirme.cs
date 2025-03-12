using makalesistemi.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace makalesistemi.Models
{
    public class Degerlendirme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MakaleId { get; set; }

        [Required]
        public int HakemId { get; set; }

        [Required]
        public string HakemDegerlendirmesi { get; set; }

        [ForeignKey("MakaleId")]
        public Makale Makale { get; set; }

        [ForeignKey("HakemId")]
        public Hakem Hakem { get; set; }
    }
}

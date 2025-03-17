using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace makalesistemi.Models
{
    public class Anonimlestirme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MakaleId { get; set; }

        [Required]
        public string DosyaYolu { get; set; }

        [ForeignKey("MakaleId")]
        public Makale Makale { get; set; }
    }
}

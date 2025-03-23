using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makalesistemi.Models
{
    public class Sohbet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Icerik { get; set; }

        [Required]
        public int GondericiId { get; set; } // Gönderen kullanıcı ID'si

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Now;

        [ForeignKey("GondericiId")]
        public Yazar Gonderici { get; set; }

        public int MakaleId { get; set; } // Mesajın hangi makaleye ait olduğu

        [ForeignKey("MakaleId")]
        public Makale Makale { get; set; }
    }
}
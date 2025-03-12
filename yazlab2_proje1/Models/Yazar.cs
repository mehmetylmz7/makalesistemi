using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makalesistemi.Models
{
    public class Yazar
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Eposta { get; set; }

        public List<Makale> Makaleler { get; set; } = new();
    }
}

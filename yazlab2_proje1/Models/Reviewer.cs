using System;
using System.Collections.Generic;

namespace makalesistemi.Models
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ExpertiseField { get; set; } = string.Empty; // Hakemin uzmanlık alanı
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<Review>? Reviews { get; set; }
    }
}

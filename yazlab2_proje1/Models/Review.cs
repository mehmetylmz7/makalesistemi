using makalesistemi.Models;
using System;

namespace makalesistemi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int ReviewerId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public int Score { get; set; } // 1 - 10 arasında puan
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Article? Article { get; set; }
        public Reviewer? Reviewer { get; set; }
    }
}
